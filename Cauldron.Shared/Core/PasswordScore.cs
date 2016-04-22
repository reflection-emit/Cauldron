namespace Cauldron.Core
{
    /// <summary>
    /// Describes the password strength as a scoring system
    /// </summary>
    public enum PasswordScore
    {
        /// <summary>
        /// Password is blank
        /// </summary>
        Blank = 0,

        /// <summary>
        /// Password length is shorter than 4 characters
        /// </summary>
        VeryWeak = 1,

        /// <summary>
        /// Password is weak
        /// </summary>
        Weak = 2,

        /// <summary>
        /// Password is medium
        /// </summary>
        Medium = 3,

        /// <summary>
        /// The password is considered as strong
        /// </summary>
        Strong = 4,

        /// <summary>
        /// The password is considered as very strong
        /// </summary>
        VeryStrong = 5
    }
}