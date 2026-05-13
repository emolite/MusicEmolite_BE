using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Domain.Entities.DISTS
{
    [Table("artists", Schema = "dist")]
    public class DistArtists : BaseModel
    {
        [Column("name")]
        public string Name { get; set; }
        [Column("stage_name")]
        public string StageName { get; set; }
        [Column("country")]
        public string Country { get; set; }
    }
}
