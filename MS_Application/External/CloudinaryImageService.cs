using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.External
{
    public class CloudinaryImageService
    {
        public Cloudinary Cloudinary { get; }
        public CloudinaryImageService(Cloudinary cloudinary) { Cloudinary = cloudinary; }
    }
}
