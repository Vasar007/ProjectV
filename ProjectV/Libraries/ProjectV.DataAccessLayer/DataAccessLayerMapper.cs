using ProjectV.DataAccessLayer.Services.Jobs.Models;
using ProjectV.DataAccessLayer.Services.Tokens.Models;
using ProjectV.DataAccessLayer.Services.Users.Models;
using ProjectV.Models.Authorization;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Internal.Jobs;
using ProjectV.Models.Users;
using Riok.Mapperly.Abstractions;

namespace ProjectV.DataAccessLayer
{
    /// <summary>
    /// Compile-time source-generated mapper (Riok.Mapperly) for the data-access layer.
    /// Replaces the AutoMapper <c>DataAccessLayerMapperProfile</c> that was removed in Plan 01-12.
    /// All mapping methods are generated at compile time — zero runtime reflection.
    /// </summary>
    [Mapper]
    public sealed partial class DataAccessLayerMapper
    {
        #region Job Mappings

        /// <summary>Maps a <see cref="JobDbInfo" /> database record to a <see cref="JobInfo" /> domain model.</summary>
        /// <param name="source">The database record to map from.</param>
        /// <returns>The mapped domain model.</returns>
        public JobInfo MapToJobInfo(JobDbInfo source)
        {
            return new JobInfo(
                id: JobId.Wrap(source.Id),
                name: source.Name,
                state: source.State,
                result: source.Result,
                config: source.Config
            );
        }

        /// <summary>Maps a <see cref="JobInfo" /> domain model to a <see cref="JobDbInfo" /> database record.</summary>
        /// <param name="source">The domain model to map from.</param>
        /// <returns>The mapped database record.</returns>
        public JobDbInfo MapToJobDbInfo(JobInfo source)
        {
            return new JobDbInfo(
                id: source.Id.Value,
                name: source.Name,
                state: source.State,
                result: source.Result,
                config: source.Config
            );
        }

        #endregion

        #region User Mappings

        /// <summary>Maps a <see cref="UserDbInfo" /> database record to a <see cref="UserInfo" /> domain model.</summary>
        /// <param name="source">The database record to map from.</param>
        /// <returns>The mapped domain model.</returns>
        public UserInfo MapToUserInfo(UserDbInfo source)
        {
            return new UserInfo(
                id: UserId.Wrap(source.Id),
                userName: UserName.Wrap(source.UserName),
                password: Password.Wrap(source.Password),
                passwordSalt: source.PasswordSalt,
                creationTimeUtc: source.Ts,
                active: source.Active,
                refreshToken: source.RefreshToken is null ? null : MapToRefreshTokenInfo(source.RefreshToken)
            );
        }

        /// <summary>Maps a <see cref="UserInfo" /> domain model to a <see cref="UserDbInfo" /> database record.</summary>
        /// <param name="source">The domain model to map from.</param>
        /// <returns>The mapped database record.</returns>
        public UserDbInfo MapToUserDbInfo(UserInfo source)
        {
            return new UserDbInfo(
                id: source.Id.Value,
                userName: source.UserName.Value,
                password: source.Password.Value,
                passwordSalt: source.PasswordSalt,
                ts: source.CreationTimeUtc,
                active: source.Active,
                refreshToken: source.RefreshToken is null ? null : MapToRefreshTokenDbInfo(source.RefreshToken)
            );
        }

        #endregion

        #region Refresh Token Mappings

        /// <summary>Maps a <see cref="RefreshTokenDbInfo" /> database record to a <see cref="RefreshTokenInfo" /> domain model.</summary>
        /// <param name="source">The database record to map from.</param>
        /// <returns>The mapped domain model.</returns>
        public RefreshTokenInfo MapToRefreshTokenInfo(RefreshTokenDbInfo source)
        {
            return new RefreshTokenInfo(
                id: RefreshTokenId.Wrap(source.Id),
                userId: UserId.Wrap(source.UserId),
                tokenHash: Password.Wrap(source.TokenHash),
                tokenSalt: source.TokenSalt,
                creationTimeUtc: source.Ts,
                expiryDateUtc: source.ExpiryDate
            );
        }

        /// <summary>Maps a <see cref="RefreshTokenInfo" /> domain model to a <see cref="RefreshTokenDbInfo" /> database record.</summary>
        /// <param name="source">The domain model to map from.</param>
        /// <returns>The mapped database record.</returns>
        public RefreshTokenDbInfo MapToRefreshTokenDbInfo(RefreshTokenInfo source)
        {
            return new RefreshTokenDbInfo(
                id: source.Id.Value,
                userId: source.UserId.Value,
                tokenHash: source.TokenHash.Value,
                tokenSalt: source.TokenSalt,
                ts: source.CreationTimeUtc,
                expiryDate: source.ExpiryDateUtc
            );
        }

        #endregion
    }
}
