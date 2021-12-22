using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TecaCoreApi.Configuraciones
{
    public class CustomDataProtectorTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser> where TUser : class
    {
        private readonly ILogger<CustomDataProtectorTokenProvider<TUser>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProtectorTokenProvider{TUser}"/> class.
        /// </summary>
        /// <param name="dataProtectionProvider">The system data protection provider.</param>
        /// <param name="options">The configured <see cref="DataProtectionTokenProviderOptions"/>.</param>
        ///    <param name="logger">The system data protection provider.</param>
        public CustomDataProtectorTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<DataProtectionTokenProviderOptions> options,
            ILogger<CustomDataProtectorTokenProvider<TUser>> logger)
        {
            if (dataProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }

            _logger = logger;
            Options = options?.Value ?? new DataProtectionTokenProviderOptions();
            // Use the Name as the purpose which should usually be distinct from others
            Protector = dataProtectionProvider.CreateProtector(Name ?? "DataProtectorTokenProvider");
        }

        /// <summary>
        /// Gets the <see cref="DataProtectionTokenProviderOptions"/> for this instance.
        /// </summary>
        /// <value>
        /// The <see cref="DataProtectionTokenProviderOptions"/> for this instance.
        /// </value>
        protected DataProtectionTokenProviderOptions Options { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDataProtector"/> for this instance.
        /// </summary>
        /// <value>
        /// The <see cref="IDataProtector"/> for this instance.
        /// </value>
        protected IDataProtector Protector { get; private set; }

        /// <summary>
        /// Gets the name of this instance.
        /// </summary>
        /// <value>
        /// The name of this instance.
        /// </value>
        public string Name { get { return Options.Name; } }

        /// <summary>
        /// Generates a protected token for the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="purpose">The purpose the token will be used for.</param>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve user properties from.</param>
        /// <param name="user">The <typeparamref name="TUser"/> the token will be generated from.</param>
        /// <returns>A <see cref="Task"/> representing the generated token.</returns>
        public virtual async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var ms = new MemoryStream();
            var userId = await manager.GetUserIdAsync(user);
            _logger.LogDebug(new EventId(1, ""), "Generate token for [purpose:{purpose}] [userId:{userId}] ", purpose, userId);

            using (var writer = ms.CreateWriter())
            {
                writer.Write(DateTimeOffset.UtcNow);
                writer.Write(userId);
                writer.Write(purpose ?? "");
                string stamp = null;
                if (manager.SupportsUserSecurityStamp)
                {
                    stamp = await manager.GetSecurityStampAsync(user);
                    if (stamp == null)
                    {
                        await manager.UpdateSecurityStampAsync(user);
                        stamp = await manager.GetSecurityStampAsync(user);
                    }
                }
                writer.Write(stamp ?? "");
            }
            var protectedBytes = Protector.Protect(ms.ToArray());
            return Convert.ToBase64String(protectedBytes);
        }

        /// <summary>
        /// Validates the protected <paramref name="token"/> for the specified <paramref name="user"/> and <paramref name="purpose"/> as an asynchronous operation.
        /// </summary>
        /// <param name="purpose">The purpose the token was be used for.</param>
        /// <param name="token">The token to validate.</param>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve user properties from.</param>
        /// <param name="user">The <typeparamref name="TUser"/> the token was generated for.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous validation,
        /// containing true if the token is valid, otherwise false.
        /// </returns>
        public virtual async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            try
            {
                var actualUserId = await manager.GetUserIdAsync(user);
                _logger.LogInformation(new EventId(1, ""), "Validate token for [purpose:{purpose}] [actualUserId:{actualUserId}]", purpose, actualUserId);

                var unprotectedData = Protector.Unprotect(Convert.FromBase64String(token));
                var ms = new MemoryStream(unprotectedData);
                using (var reader = ms.CreateReader())
                {
                    var creationTime = reader.ReadDateTimeOffset();
                    var expirationTime = creationTime + Options.TokenLifespan;
                    _logger.LogDebug(new EventId(1, ""), "Validate token for [actualUserId:{actualUserId}] [creationTime:{creationTime}] - [expirationTime:{expirationTime}]", actualUserId, creationTime, expirationTime);
                    if (expirationTime < DateTimeOffset.UtcNow)
                    {
                        _logger.LogWarning(new EventId(1, ""), "Token is expired [expirationTime:{expirationTime}]", expirationTime);
                        return false;
                    }
                    var userId = reader.ReadString();
                    if (userId != actualUserId)
                    {
                        _logger.LogWarning(new EventId(1, ""), "Token is not for this user [userId:{userId}] - [actualUserId:{actualUserId}]", userId, actualUserId);
                        return false;
                    }
                    var purp = reader.ReadString();
                    if (!string.Equals(purp, purpose))
                    {
                        _logger.LogWarning(new EventId(1, ""), "Token is for wrong purp [purp:{purp}] - [purpose:{purpose}]", purp, purpose);
                        return false;
                    }
                    var stamp = reader.ReadString();
                    if (reader.PeekChar() != -1)
                    {
                        _logger.LogWarning(new EventId(1, ""), "Token stamp not valid [stamp:{stamp}]", stamp);
                        return false;
                    }
                    if (manager.SupportsUserSecurityStamp)
                    {
                        return stamp == (await manager.GetSecurityStampAsync(user) ?? "");
                    }
                    return stamp == "";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(1, ""), "Validate exception [ex:{ex}]", ex);
            }
            return false;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether a token generated by this instance
        /// can be used as a Two Factor Authentication token as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve user properties from.</param>
        /// <param name="user">The <typeparamref name="TUser"/> the token was generated for.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query,
        /// containing true if a token generated by this instance can be used as a Two Factor Authentication token, otherwise false.
        /// </returns>
        /// <remarks>This method will always return false for instances of <see cref="DataProtectorTokenProvider{TUser}"/>.</remarks>
        public virtual Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult(false);
        }
    }
}