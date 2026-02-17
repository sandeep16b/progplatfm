namespace Abim.Platform.Program.WebApi.Api.Constants
{
    /// <summary>
    /// Constants for AntiXss allowed strings
    /// </summary>
    public static class AntiXssConstants
    {
        /// <summary>
        /// The rich text tags
        /// </summary>
        /// <remarks>
        /// Do not remove the spaces in these strings
        /// </remarks>
        public const string RichTextTags =  /*Bold*/            "<b>,<b ,</b>,<b/," +
                                            
                                            /*Italic*/          "<i>,<i ,</i>,<i/," +
                                            
                                            /*Underline*/       "<u>,<u ,</u>,<u/," +
                                            
                                            /*Unordered list*/  "<ul>,<ul ,</ul>,<ul/," +
                                            
                                            /*Ordered list*/    "<ol>,<ol ,</ol>,<ol/," +
                                            
                                            /*List item*/       "<li>,<li ,</li>,<li/," +
                                            
                                            /*Paragraph*/       "<p>,<p ,</p>,<p/," +
                                            
                                            /*Strong*/          "<strong>,<strong ,</strong>,<strong/," +
                                            
                                            /*Emphasized*/      "<em>,<em ,</em>,<em/," +
                                            
                                            /*Division*/        "<div>,<div ,</div>,<div/," +
                                            
                                            /*Line Break*/      "<br>,<br ,</br>,<br/," +
                                            
                                            /*Font*/            "<font>,<font ,</font>,<font/," +
                                            
                                            /*Span*/            "<span>,<span ,</span>,<span/," +
                                            
                                            /*Anchor*/          "<a>,<a ,</a>,<a/," +
                                            
                                            /*Style*/           "<style>,<style ,</style>,<style/," +
                                            
                                            /*Image*/           "<img>,<img ,</img>,<img/," +
                                            
                                            /*(General)*/       ">,',’,\"";
        
        /// <summary>
        /// The basic tags
        /// </summary>
        public const string BasicTags = "',’,\"";
        
        /// <summary>
        /// The banned strings
        /// </summary>
        public const string BannedStrings = "javascript,vbscript";

        /// <summary>
        /// Validates that no banned strings are included.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static bool ValidateNoBannedStrings(string text)
        {
            var lower = text.ToLower();
            foreach(var banned in BannedStrings.Split(','))
                if(lower.Contains(banned)) return false;
            return true;
        }
    }
}
