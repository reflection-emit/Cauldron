using System;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace Cauldron
{
    /// <summary>
    /// Provides usefull extensions for the directory services
    /// </summary>
    public static class ExtensionsDirectoryServices
    {
        /// <summary>
        /// Impersonates the given user
        /// </summary>
        /// <param name="principalContext">The principal context of the user</param>
        /// <param name="username">The user name of the user to impersonate</param>
        /// <param name="password">The password of the user to impersonate</param>
        /// <param name="logonType">The type of logon operation to perform.</param>
        /// <returns>A <see cref="WindowsImpersonationContext"/> of the impersonation</returns>
        /// <exception cref="ArgumentNullException"><paramref name="username"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="username"/> is empty</exception>
        /// <exception cref="ArgumentException"><paramref name="password"/> is empty</exception>
        /// <example>
        /// <code>
        /// using (var context = new PrincipalContext(ContextType.Domain))
        /// {
        ///     context.Impersonate("billgates", "superSafePassword!!!!1111", LogonType.Network);
        ///
        ///     // Do anything in the context of the user 'billgates'
        ///     this.database.DeleteAll();
        /// }
        /// </code>
        /// </example>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static WindowsImpersonationContext Impersonate(this PrincipalContext principalContext, string username, string password, LogonType logonType)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username));

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("The parameter cannot be empty", nameof(username));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("The parameter cannot be empty", nameof(password));

            SafeTokenHandle handle;

            if (!UnsafeNative.LogonUser(username, principalContext.Name, password, (int)logonType, 3 /* LOGON32_PROVIDER_WINNT50 */, out handle))
                throw new PrincipalOperationException("Could not impersonate the user.", Marshal.GetLastWin32Error());

            var identity = WindowsIdentity.Impersonate(handle.DangerousGetHandle());
            handle.Dispose();

            return identity;
        }
    }
}