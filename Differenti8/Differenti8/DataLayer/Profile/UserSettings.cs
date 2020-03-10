using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Differenti8.DataLayer.Profile
{
    /// <summary>
    /// Current user settings class.
    /// </summary>
    /// <remarks>
    /// This is a persistable type safe collection of user setting objects.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class UserSettings : ProfileEntries<UserSetting>
    {
        public ProfileCache ProfileCache { get; set; }

        public UserSettings(ProfileCache profileCache)
            : this(null, profileCache)
        {
        }

        public UserSettings(string fileSpec, ProfileCache profileCache)
            : base("Profile", "UserSettings", "UserSetting", "Key", fileSpec, null)
        {
            ProfileCache = profileCache;
        }

        public override void UpdateAssignments(UserSetting entry)
        {
            UpdateItem.NewBaseDir = entry.NewBaseDir;
            UpdateItem.OldBaseDir = entry.OldBaseDir;
            UpdateItem.NewFile = entry.NewFile;
            UpdateItem.OldFile = entry.OldFile;
            UpdateItem.KeyShrunk = entry.KeyShrunk;
            UpdateItem.MinChars = entry.MinChars;
            UpdateItem.MinLines = entry.MinLines;
            UpdateItem.LimitCharacters = entry.LimitCharacters;
            UpdateItem.LimitLines = entry.LimitLines;
            UpdateItem.SubLineMatchLimit = entry.SubLineMatchLimit;
            UpdateItem.CompleteLines = entry.CompleteLines;
        }
    }
}