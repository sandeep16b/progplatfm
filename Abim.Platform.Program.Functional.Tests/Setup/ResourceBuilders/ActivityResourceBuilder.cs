using Abim.Platform.Product.Extensions.ExternalResponses;
using Abim.Platform.Product.Resources;
using Abim.Platform.Product.Resources.Enums;
using System;

namespace Abim.Platform.Program.Tests.Setup.ResourceBuilders
{
    public class ActivityResourceBuilder
    {
        private ActivityResource _activity;

        public ActivityResourceBuilder()
        {
            _activity = new ActivityResource();
        }

        public ActivityResourceBuilder WithCompletedDate(DateTime? completedDate)
        {
            _activity.CompletedDate = completedDate;
            return this;
        }

        public ActivityResourceBuilder WithActivityResult(ActivityResultType result)
        {
            _activity.ActivityResult = 
                new EnumValueResponseResource<ActivityResultType>(result);

            return this;
        }

        public ActivityResourceBuilder WithTotalMOCPoints(decimal points)
        {
            _activity.TotalMOCPoints = points;
            return this;
        }

        public ActivityResourceBuilder WithProduct(ProductResource product)
        {
            _activity.Product = product;
            return this;
        }

        public ActivityResource Build()
        {
            var output = _activity;
            _activity = new ActivityResource();
            return output;
        }
    }
}
