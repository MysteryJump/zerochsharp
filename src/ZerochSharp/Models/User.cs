using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZerochSharp.Models
{
    public class User
    {
        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [Required]
        [JsonRequired]
        public string UserId { get; set; }

        [JsonIgnore]
        [Required]
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
        //[JsonIgnore]
        [JsonIgnore]
        public string PasswordSalt { get; set; }
        public SystemAuthority SystemAuthority { get; set; }
        [JsonIgnore]
        public CapAuthority CapAuthority { get; set; }
        [JsonIgnore]
        public string ControllableBoard { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [NotMapped]
        public string[] ControllableBoards
        {
            get
            {
                return ControllableBoard?.Split(';', StringSplitOptions.RemoveEmptyEntries);
            }
            set
            {
                if (value.Length > 100)
                {
                    ControllableBoard = value.Aggregate(new StringBuilder(), (before, current) => before.Append(current).Append(";")).ToString();
                }
                else
                {
                    ControllableBoard = value.Aggregate("", (before, current) => before + current + ";");
                }
            }
        }

        public bool IsValidUserName()
        {
            var regex = new Regex(@"^[a-zA-Z0-9\-_]{4,20}$", RegexOptions.Compiled);
            return regex.IsMatch(UserId);
        }


        public bool HasSystemAuthority(SystemAuthority authority, string boardKey = null)
        {
            // authority_map.png
            if ((SystemAuthority & SystemAuthority.Owner)
                == SystemAuthority.Owner)
            {
                return true;
            }
            if ((SystemAuthority & SystemAuthority.Admin)
                == SystemAuthority.Admin && authority != SystemAuthority.Owner)
            {
                return true;
            }
            if ((SystemAuthority & SystemAuthority.BoardsManagement)
                == SystemAuthority.BoardsManagement)
            {
                if (InnerBoardAuthority(authority) ||
                    authority == SystemAuthority.BoardsManagement)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if ((SystemAuthority & SystemAuthority.RemoveResponse) == SystemAuthority.RemoveResponse)
            {
                if (authority == SystemAuthority.AboneResponse 
                    || authority == SystemAuthority.RemoveResponse)
                {
                    if (!string.IsNullOrEmpty(boardKey))
                    {
                        return ControllableBoards?.Any(x => x == boardKey) ?? false;
                    }
                }
            }
            if ((SystemAuthority & SystemAuthority.CapUserSetting) == SystemAuthority.CapUserSetting 
                && authority == SystemAuthority.CapUserSetting)
            {
                return true;
            }
            if (InnerBoardAuthority(authority) 
                && !string.IsNullOrEmpty(boardKey) 
                && ((SystemAuthority & authority) == authority || (SystemAuthority & SystemAuthority.BoardSetting) == SystemAuthority.BoardSetting))
            {
                return ControllableBoards?.Any(x => x == boardKey) ?? false;
            }
            return false;
        }
        /// <summary>
        /// Check the authority is only avalilable in specified board. (This method only support unit authority)
        /// </summary>
        /// <param name="authority">System authority</param>
        /// <returns>Avalilable or not</returns>
        private bool InnerBoardAuthority(SystemAuthority authority)
        {
            return authority == SystemAuthority.ThreadArchive ||
                authority == SystemAuthority.ThreadStop ||
                authority == SystemAuthority.EditResponse ||
                authority == SystemAuthority.ViewResponseDetail ||
                authority == SystemAuthority.BoardSetting ||
                authority == SystemAuthority.RemoveResponse ||
                authority == SystemAuthority.AboneResponse;
        }
        public bool HasCapAuthority(params CapAuthority[] authority)
        {
            foreach (var item in authority)
            {
                if ((item & CapAuthority) != item)
                {
                    return false;
                }
            }
            return true;
        }
    }
    

    [Flags]
    public enum CapAuthority
    {

    }
    /// <summary>
    /// System authority flag. See also <image url="authority_map.png"></image>
    /// </summary>
    [Flags]
    public enum SystemAuthority
    {
        // Admin, CapUserSetting (at the moment), Owner, BoardsManagement is available in all boards. others is not always avalilabe  
        Admin = 1 << 0, // System administrator (root user), this user has all below authority without owner authority
        ThreadStop = 1 << 1, // TODO: this function is not implemented
        ThreadArchive = 1 << 2, // TODO: this function is not implemented
        BoardSetting = 1 << 3, // TODO: ACL in React App, to change plugin enabled/disabled in respective board setting page
        CapUserSetting = 1 << 4, // TODO: implement cap user function
        AboneResponse = 1 << 5,
        EditResponse = 1 << 6,
        RemoveResponse = 1 << 7,
        ViewResponseDetail = 1 << 8,
        Owner = 1 << 9, // System administrator 
                        // (contains root user authority, this user is unique and cannot delete by other user including own, 
                        // but owner authority can transfer to other user)
        BoardsManagement = 1 << 10, // this authority contains BoardSetting
    }
}
