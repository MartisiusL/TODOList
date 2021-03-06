using TODOList.Entities;

namespace TODOList.Helpers
    {
    public static class ExtensionMethods
        {

        public static User WithoutPassword (this User user)
            {
            if (user == null) return null;

            user.PasswordHash = null;
            return user;
            }
        }
    }
