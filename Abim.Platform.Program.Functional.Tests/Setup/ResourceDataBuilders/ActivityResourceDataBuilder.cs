using Abim.Platform.Product.Resources;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Product.Resources.Enums;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Setup.ResourceDataBuilders
{
    public class ActivityResourceDataBuilder : Enterprise.Core.Testing.Setup.DataBuilders.ResourceDataBuilder<ActivityResource, ActivityResourceDataBuilder>
    {
        // private ActivityResource _activity;

        public ActivityResourceDataBuilder(ActivityResource activity) : base(activity)
        {
        }

        public ActivityResourceDataBuilder() : base(() => GetDataCreator())
        {
        }

        public static ActivityResource GetDataCreator()
        {
            return new ActivityResource();
        }

        public ActivityResourceDataBuilder WithPoints (DateTime ActivityCompletedDate, decimal TotalMOCPoints = 100)
        {
            With(a => a.Product = new ProductResource() { Code = "Any" })
                .With(a => a.CompletedDate = ActivityCompletedDate)
                .With(a => a.ActivityResult = new Product.Extensions.ExternalResponses.EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass))
                .With(a => a.TotalMOCPoints = TotalMOCPoints)
                .With(a => a.ActivityCredits = new List<ActivityCreditResource>()
                    {
                    //-- MedicalKnowledgePoints
                    new ActivityCreditResource()
                        {
                            CreditDate = ActivityCompletedDate,
                            CreditType = new CreditTypeResource() { Value = ProductResourceConstants.CreditTypeValue.MedicalKnowledgePoints },
                            Claimed=true,
                            CreditEarned = TotalMOCPoints/2
                        },
                    //-- PracticeAssessement
                    new ActivityCreditResource()
                        {
                            CreditDate = ActivityCompletedDate,
                            CreditType = new CreditTypeResource() { Value = ProductResourceConstants.CreditTypeValue.PracticeAssessement },
                            Claimed=true,
                            CreditEarned = TotalMOCPoints/2
                        }
                    }
                );

            return this;
        }

        public ActivityResourceDataBuilder WithReciprocity(DateTime ActivityCompletedDate)
        {
            With(a => a.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ReciprocityAttest })
                .With(a => a.CompletedDate = ActivityCompletedDate)
                .With(a => a.ActivityResult = new Product.Extensions.ExternalResponses.EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass));

            return this;
        }

        public ActivityResourceDataBuilder WithAttestation(DateTime ActivityCompletedDate, string ProductCode)
        {
            //var targetAttestationType = ProductCode ==  ProgramResourceConstants.CertificationCode.InterventionalCardiology ? 
            //            ProductResourceConstants.ProductCode.ICARDAttestMOC
            //       :    ProductResourceConstants.ProductCode.FPHMAttestMOC;

            With(a => a.Product = new ProductResource() { Code = ProductCode })
                .With(a => a.CompletedDate = ActivityCompletedDate)
                .With(a => a.ActivityResult = new Product.Extensions.ExternalResponses.EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass));

            return this;
        }

    }
}
