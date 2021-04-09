using ANFAPP.Logic.Database.DAOs;
using ANFAPP.Logic.Database.Models;
using ANFAPP.Logic.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.ViewModels
{
    public class UsersViewModel : IViewModel
    {

        #region Events

        public OnSuccessEventHandler OnInsertSuccess;

        #endregion

        #region Properties

        protected UserDAO _db = new UserDAO();

        #endregion

        #region DB Loaders

        /// <summary>
        /// Insert the new user into the database.
        /// </summary>
        public async void InsertNewUser(bool isMale)
        {
            // Insert the new user into the database.
            await _db.Insert(new User()
            {
                IsMale = isMale
            });

            // Get the list of users.
            var users = await _db.GetAll();

            // Get the first user, if exists.
            if (users != null && users.Count > 0) SessionData.BiometricUser = users[0];

            // Trigger event
            if (OnInsertSuccess != null) OnInsertSuccess();
        }

        /// <summary>
        /// Loads the first user into cache.
        /// </summary>
        public async Task LoadDataAsync()
        {
            // Clear cached user.
            SessionData.BiometricUser = null;

            // Get the list of users.
            var users = await _db.GetAll();

            // Get the first user, if exists.
            if (users != null && users.Count > 0) SessionData.BiometricUser = users[0];
        }

        #endregion

    }
}
