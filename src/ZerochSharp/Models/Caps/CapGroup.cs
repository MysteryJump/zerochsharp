using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZerochSharp.Models.Caps
{
    public class CapGroup
    {
        [Key]
        public int Id { get; set; }
        public string CapGroupName { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsCommonGroup { get; set; }

        #region Authorities
        [NotMapped]
        public bool DeregulationOfTitleLengthViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfMailLengthViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfNameLengthViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfBodyLengthViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfBodyLineLengthViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfBodyLineCountViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfNoNameViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfCreatingThreadViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfCreatingThreadOnCapOnlyViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfRepeatingResponseViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfDuplicatedResponseViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfDurationViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfReadOnlyViolation { get; set; }
        [NotMapped]
        public bool IsUsingExclusiveCapId { get; set; }
        [NotMapped]
        public bool IsHiddenAuthorId { get; set; }
        [NotMapped]
        public bool IsHiddenTerminalId { get; set; }
        [NotMapped]
        public bool IsHiddenBodyHost { get; set; }
        [NotMapped]
        public bool DeregulationOfCreatingThreadFromLegacyPhoneViolation { get; set; }
        [NotMapped]
        public bool IsShowingCapNameViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfSambaViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfProxyViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfForeignHostViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfRestrictedUserViolation { get; set; }
        [NotMapped]
        public bool DeregulationOfProhibitedWordsViolation { get; set; }


        #endregion

    }
}
