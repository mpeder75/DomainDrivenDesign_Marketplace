using Marketplace.Domain.Entities;
using Marketplace.Domain.ValueObjects;

namespace Marketplace.Domain.Services;

    public static class PictureRules
    {
        public static bool HasCorrectSize(this Picture picture)
            => picture != null 
               && picture.Size.Width >= 800 
               && picture.Size.Height >= 600;
    }
