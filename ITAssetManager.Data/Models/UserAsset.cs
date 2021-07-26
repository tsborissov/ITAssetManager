using System;

namespace ITAssetManager.Data.Models
{
    public class UserAsset
    {
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int AssetId { get; set; }

        public Asset Asset { get; set; }

        public DateTime AssignDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}
