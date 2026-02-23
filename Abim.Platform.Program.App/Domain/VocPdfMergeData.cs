using System;
using System.Collections.Generic;
using System.Text;

namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// Pdf Merge Class for ProfileResource class.
    /// </summary>
    public class VocPdfMergeData : IPdfMergeData
    {
        private const string style10ptTimes = "style=\"font-size:9pt;font-family:times;\"";

        VocPdfData mergeData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_mergeData"></param>
        public VocPdfMergeData(VocPdfData _mergeData)
        {
            mergeData = _mergeData;
        }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> MergeFieldValues
        {
            get { return getMergeDictionary(); }
        }
        IDictionary<string, string> getMergeDictionary()
        {
            var output = new Dictionary<string, string>();
            output.Add("Name", mergeData.Name);
            output.Add("Datetime", DateTime.Now.ToString("MMMM dd, yyyy"));
            output.Add("Address", mergeData.Address);
            output.Add("InitialCerts", mergeData.IntialCertifications);
            output.Add("MaintainingMOC", mergeData.isMaintainingMOC);
            output.Add("Certifications", mergeData.CurrentCertifications);

            var letter = 
                $"{HtmlLetterBody(mergeData.Text)}" +
                $"{HtmlLetterVerificationDetail(mergeData)}";

            output.Add("LetterBody", letter);

            output.Add("Certified", mergeData.Certified);
            return output;
        }        

        private string HtmlLetterBody(string certifiedDate)
        {
            StringBuilder body = new StringBuilder();
            
            body.Append($"<p {style10ptTimes}>Below is the certification status of this physician as of {certifiedDate}. In addition to reporting board certification, ABIM " +
                $"reports whether or not ABIM Board Certified physicians are participating in Maintenance of Certification (MOC). Because ABIM's MOC program requires continuous " +
                $"activities to maintain certification, we no longer issue certificates with end dates; however, we recognize that some entities may still require current " +
                $"certification status to complete primary-source verification.</p>");
            body.Append($"<br {style10ptTimes}>&nbsp;");
            body.Append($"<p {style10ptTimes}>Our website, <u>www.abim.org/verify</u>, is the primary verification source for ABIM Board Certification. We will update this status on an annual basis. " +
                "Therefore, we encourage credentialers to use the " +
                "<b>ABIM's annual re-verification date of April 1</b>" +
                " to verify your physician's status. By using the annual re-verification date suggested, you will have accurate information about a physician's current " +
                "certification status. Visit " +
                "<u>www.abim.org/moc</u>" + " to review ABIM's Maintenance of Certification policies.</b></p>");
            body.Append($"<br {style10ptTimes}>&nbsp;");
            body.Append($"<p {style10ptTimes}>If you have questions, please call 1 (800) 441-ABIM (2246), Mon. - Fri., 8:30 a.m. to 6 p.m. ET, or email us at request@abim.org.</p>");

            return body.ToString();
        }

        private string HtmlLetterVerificationDetail(VocPdfData data) {
            StringBuilder body = new StringBuilder();

            var initCertText = data.IntialCertifications
                .Replace(Environment.NewLine, "<br>");

            body.Append($"<br {style10ptTimes}>&nbsp;<br {style10ptTimes}>&nbsp;");
            body.Append($"<p {style10ptTimes}>Name: <b>{data.Name}</b></p>");
            body.Append($"<br {style10ptTimes}>&nbsp;");

            if (!data.isCertified)
            {
                body.Append($"<p {style10ptTimes}><b>Not Certified</b></p>");
            }
            else if (!data.isActive)
            {
                body.Append($"<p {style10ptTimes}><b>Inactive</b></p>");
                body.Append($"<br {style10ptTimes}>&nbsp;");
                body.Append($"<p {style10ptTimes}>Physicians are publicly reported as inactive if they were once certified by ABIM but now, for " +
                    $"non-disciplinary reasons, they no longer have an active medical license in any jurisdiction.</p>");
            }
            else
            {
                // certified
                var currentCertsText = data.CurrentCertifications
                    .Replace(": Certified", ": <b>Certified</b>")
                    .Replace(Environment.NewLine, "<br>");

                body.Append($"<p {style10ptTimes}>{currentCertsText}</p>");
                body.Append($"<br {style10ptTimes}>&nbsp;");
                body.Append($"<p {style10ptTimes}>Participating in MOC: <b>{data.isMaintainingMOC}</b></p>");
            }

            body.Append($"<br {style10ptTimes}>&nbsp;");
            body.Append($"<p {style10ptTimes}>{initCertText}</p>");

            return body.ToString();
        }


    }
}
