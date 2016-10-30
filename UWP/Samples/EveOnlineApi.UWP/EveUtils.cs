using Cauldron.Activator;
using EveOnlineApi.Models;
using EveOnlineApi.WebService;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#if WINDOWS_UWP

using Windows.UI.Xaml.Documents;

#else

#endif

namespace EveOnlineApi
{
    public static class EveUtils
    {
        public async static Task<DateTime> GetEveTimeAsync()
        {
            try
            {
                var eve = Factory.Create<IEveApi>();
                var result = await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, EveTime>((await eve.GetMotdAsync()).Time.HRef);
                return result.Time;
            }
            catch
            {
                var info = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
                return TimeZoneInfo.ConvertTime(DateTimeOffset.Now, info).DateTime;
            }
        }

        public async static Task<long> GetOnlinePlayersAsync()
        {
            try
            {
                var server = Factory.Create<IEveApi>().Server;
                var result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, ServerStatus>($"{server.XMLApi}server/ServerStatus.xml.aspx");
                return result.OnlinePlayers;
            }
            catch
            {
                return 0;
            }
        }

        public async static Task<EveServerStatus> GetServerStatusAsync()
        {
            try
            {
                EveServers server = Factory.Create<IEveApi>().Server;
                ServerStatus result = await WebServiceConsumer.Consume<DefaultRestClient, XmlDeserializer, ServerStatus>($"{server.XMLApi}server/ServerStatus.xml.aspx");
                return result.ServerOpen ? EveServerStatus.Online : EveServerStatus.Offline;
            }
            catch
            {
                return EveServerStatus.Unknown;
            }
        }

        public async static Task<string> GetServerVersionAsync()
        {
            var server = Factory.Create<IEveApi>().Server;
            var result = await WebServiceConsumer.Consume<DefaultRestClient, JsonDeserializer, MotdCachingAgent, Motd>(server.Crest);
            return result.ServerVersion;
        }

        public static string PrettifyMailBody(string body)
        {
            var html = @"<html xmlns = ""http://www.w3.org/1999/xhtml"" class="""">" +
                "<head><style>" +
                @"/* MAIL CSS */
				.eveFontSize8  { font-size: 10px;}
				.eveFontSize9  { font-size: 11px;}
				.eveFontSize10 { font-size: 12px;}
				.eveFontSize11 { font-size: 13px;}
				.eveFontSize12 { font-size: 14px;}
				.eveFontSize14 { font-size: 15px;}
				.eveFontSize18 { font-size: 16px;}
				.eveFontSize24 { font-size: 18px;}
				.eveFontSize30 { font-size: 20px;}
				.eveFontSize36 { font-size: 24px;}

				.eveFontffffff { color: White;}
				.eveFontb2b2b2 { color: Silver;}
				.eveFont4c4c4c { color: Gray;}
				.eveFont000000 { color: Black;}
				.eveFontffff00 { color: Yellow;}
				.eveFont00ff00 { color: Green;}
				.eveFontff0000 { color: Red;}
				.eveFont0000ff { color: Blue;}
				.eveFont7f7f00 { color: #7f7f00;} /* dark yellow */
				.eveFont007f00 { color: #007f00;} /* dark green */
				.eveFont7f0000 { color: #7f0000;} /* dark red */
				.eveFont00007f { color: #00007f;} /* dark blue */
				.eveFont7f007f { color: #7f007f;} /* dark magenta */
				.eveFont00ffff { color: #00ffff;} /* cyan */
				.eveFontff00ff { color: #ff00ff;} /* magenta */
				.eveFont007fff { color: #007fff;} /* light blue */

				a, a.selected
				{
					outline: none;
					color: #fa9e0e;
				}
				a:hover
				{
					text-decoration: underline;
				}
				body
				{
					margin: 0; /* it's good practice to zero the margin and padding of the body element to account for differing browser defaults */
					padding: 10;
					background: #292929;
					width: 90%;
					font: 100% Segoe UI, Arial, Helvetica, sans-serif;
					text-align: left;
					color: #ffffff;
				}

				.cleared {
					clear: both;
					height: 0px;
					width: 0px;
				}" +
                "</style></head><body>" + body
                .Replace("<font size=\"", "<span class=\"eveFontSize")
                .Replace("\" color=\"#ff", " eveFont")
                .Replace("\" color=\"#ef", " eveFont")
                .Replace("\" color=\"#df", " eveFont")
                .Replace("\" color=\"#cf", " eveFont")
                .Replace("\" color=\"#bf", " eveFont")
                .Replace("\" color=\"#af", " eveFont")
                .Replace("</font>", "</span>")
                .Replace("href=\"showinfo:", "href=\"eveonline:")
                .Replace("href=\"fitting:", "href=\"eveonlinefitting:")
                .Replace("href=\"", "target=\"_blank\" href=\"") + " </body></html>";

            return html;
        }

        public static string XAMLify(string text)
        {
            var xaml = "<Inline>" + text + "</Inline>";

            xaml = Regex.Replace(xaml, @"<color='0x(.*?)'>", x =>
            {
                var codeString = x.Groups[1].Value;
                return $"<Span Foreground=\"#{codeString}\">";
            }).Replace("</color>", "</Span>");

            xaml = Regex.Replace(xaml, @"<font size=\\""(.*?)\\"">", x =>
            {
                var codeString = x.Groups[1].Value;
                return $"<Span FontSize=\"{codeString}\">";
            }).Replace("</font>", "</Span>");

            xaml = Regex.Replace(xaml, @"<font color=\\""#(.*?)\\"">", x =>
            {
                var codeString = x.Groups[1].Value;
                return $"<Span Foreground=\"#{codeString}\">";
            }).Replace("</font>", "</Span>");

#if WINDOWS_UWP
            xaml = Regex.Replace(xaml, @"<a href=showinfo:(.*?)>", x =>
            {
                var codeString = x.Groups[1].Value;
                return $"<Hyperlink a:HyperlinkProperties.NavigateUri=\"eveonline:{codeString}\">";
            }).Replace("</a>", "</Hyperlink>");

            xaml = Regex.Replace(xaml, @"<url=showinfo:(.*?)>", x =>
            {
                var codeString = x.Groups[1].Value;
                return $"<Hyperlink a:HyperlinkProperties.NavigateUri=\"eveonline:{codeString}\">";
            }).Replace("</url>", "</Hyperlink>");

            xaml = Regex.Replace(xaml, @"<url=(.*?)>", x =>
            {
                var codeString = x.Groups[1].Value;
                return $"<Hyperlink a:HyperlinkProperties.NavigateUri=\"{codeString}\">";
            }).Replace("</url>", "</Hyperlink>");
#else
            xaml = Regex.Replace(xaml, @"<a href=showinfo:(.*?)>", x =>
            {
                var codeString = x.Groups[1].Value;
                return $"<Hyperlink a:HyperlinkProperties.NavigateUri=\"eveonline1:{codeString}\">";
            }).Replace("</a>", "</Hyperlink>");

            xaml = Regex.Replace(xaml, @"<url=showinfo:(.*?)>", x =>
            {
                var codeString = x.Groups[1].Value;
                return $"<Hyperlink a:HyperlinkProperties.NavigateUri=\"eveonline1:{codeString}\">";
            }).Replace("</url>", "</Hyperlink>");

            xaml = Regex.Replace(xaml, @"<url=(.*?)>", x =>
            {
                var codeString = x.Groups[1].Value;
                return $"<Hyperlink a:HyperlinkProperties.NavigateUri=\"{codeString}\">";
            }).Replace("</url>", "</Hyperlink>");
#endif

            return xaml
                .Replace("\r\n", " ")
                .Replace(@"\""", "'")
                .Replace("</b>", "</Bold>")
                .Replace("<b>", "<Bold>")
                .Replace("</i>", "</Italic>")
                .Replace("<i>", "<Italic>");
        }
    }
}