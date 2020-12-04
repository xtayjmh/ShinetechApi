using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.Data
{
    public class Parameter : BaseEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(User.CreatedParameters))]
        public User CreatedByPerson { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        [InverseProperty(nameof(User.UpdatedParameters))]
        public User UpdateByPerson { get; set; }
    }
}
