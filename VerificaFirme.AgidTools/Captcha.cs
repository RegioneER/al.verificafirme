using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;

namespace RER.Tools.MVC.Agid
{
    public static class Captcha
    {
        //public enum CaptchaType { Math, Image };

        //public static bool VerificaCaptcha(string captchaValue, string CaptchaEncriptedCode, CaptchaType type = CaptchaType.Math)
        //{
        //    if (type == CaptchaType.Math)
        //    {
        //        string captchaDecriptedCode = RER.Tools.DesCryptor.DecryptText(CaptchaEncriptedCode, (string)Microsoft.Win32.Registry.LocalMachine.OpenSubKey(RER.Tools.Configuration.RootRegKey).GetValue("Test"));
        //        return captchaValue.Length == 6 && captchaDecriptedCode.EndsWith($"={captchaValue}");
        //    }
        //    else
        //    {
        //        string captchaDecriptedCode = RER.Tools.DesCryptor.DecryptText(CaptchaEncriptedCode, (string)Microsoft.Win32.Registry.LocalMachine.OpenSubKey(RER.Tools.Configuration.RootRegKey).GetValue("Test"));
        //        return captchaValue.Length == 6 && captchaDecriptedCode.Contains(captchaValue);
        //    }
        //}

        public static bool VerificaTokenReCaptcha(HttpRequestBase httpRequest, string secret)
        {
            string g_recaptcha_response = httpRequest.Params["g-recaptcha-response"]?.ToString();

            if (string.IsNullOrWhiteSpace(g_recaptcha_response))
                return false;

            RestClient restClient = new RestClient("https://www.google.com/recaptcha/api/");
            var restRequest = new RestRequest("siteverify", Method.POST, DataFormat.Json);
            restRequest.AddParameter("secret", secret);
            restRequest.AddParameter("response", g_recaptcha_response);
            restRequest.AddParameter("remoteip", httpRequest.UserHostAddress);

#if (DEBUG)
            System.Net.WebProxy proxy = new System.Net.WebProxy("vm445lnx.ente.regione.emr.it", 3128);
            proxy.Credentials = new NetworkCredential("Majowski_M", "MettiQuiLaTuaPassword", "RERSDM");
            restClient.Proxy = proxy;
#endif

            var response = restClient.Execute(restRequest);

            dynamic data = Json.Decode((response?.Content) ?? "");
            return response.IsSuccessful && data.success == true;
        }
    }


    public static partial class ExtensionMethods
    {
        public static MvcHtmlString reCaptcha<TModel>(this HtmlHelper<TModel> helper, string siteKey)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddAttribute("data-sitekey", siteKey);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "g-recaptcha");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.RenderEndTag(); // end Input
            }
            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString reCaptchaScript<TModel>(this HtmlHelper<TModel> helper)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddAttribute("defer", "");
                writer.AddAttribute(HtmlTextWriterAttribute.Src, "https://www.google.com/recaptcha/api.js");
                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                writer.RenderEndTag(); // end Input
            }
            return new MvcHtmlString(stringWriter.ToString());
        }


        //using static RER.Tools.MVC.Agid.Captcha;

        //private static string GetCaptchaImageCode()
        //{
        //    StringWriter stringWriter = new StringWriter();

        //    Random random = new Random();
        //    string combination = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        //    StringBuilder captcha = new StringBuilder();
        //    for (int i = 0; i < 6; i++)
        //        captcha.Append(combination[random.Next(combination.Length)]);

        //    return captcha.ToString();
        //}

        //private static string GetCaptchaMathCode()
        //{
        //    StringWriter stringWriter = new StringWriter();

        //    Random random = new Random();
        //    string numbers = "0123456789";
        //    string operators = "+-*/";
        //    StringBuilder captcha = new StringBuilder();

        //    int nr1 = int.Parse(numbers[random.Next(numbers.Length)].ToString());
        //    int nr2 = int.Parse(numbers[random.Next(numbers.Length)].ToString());
        //    string op = operators[random.Next(operators.Length)].ToString();

        //    int result = 0;
        //    switch (op)
        //    {
        //        case "+":
        //            result = nr1 + nr2;
        //            break;
        //        case "-":
        //            result = nr1 - nr2;
        //            break;
        //        case "*":
        //            result = nr1 * nr2;
        //            break;
        //        case "/":
        //            int tmpVal = nr1 * nr2;
        //            nr1 = tmpVal;
        //            result = nr1 / nr2;
        //            break;
        //    }

        //    captcha.Append(nr1);
        //    captcha.Append(op);
        //    captcha.Append(nr2);
        //    captcha.Append($"={result}");

        //    return captcha.ToString();
        //}

        //private static MvcHtmlString CaptchaLabel<TModel>(this HtmlHelper<TModel> helper)
        //{
        //    StringWriter stringWriter = new StringWriter();
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
        //        {
        //            writer.AddAttribute(HtmlTextWriterAttribute.Id, $"captchaLabel");
        //            writer.AddAttribute(HtmlTextWriterAttribute.For, "captcha");
        //            writer.AddAttribute(HtmlTextWriterAttribute.Class, "control-label");
        //            writer.RenderBeginTag(HtmlTextWriterTag.Label);
        //            writer.Write("Codice di verifica");
        //            writer.RenderEndTag(); // end Label
        //        }
        //        return new MvcHtmlString(stringWriter.ToString());
        //    }
        //}

        //private static MvcHtmlString CaptchaTextBox<TModel>(this HtmlHelper<TModel> helper)
        //{
        //    StringWriter stringWriter = new StringWriter();
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
        //        {
        //            writer.AddAttribute(HtmlTextWriterAttribute.Id, $"captcha");
        //            writer.AddAttribute(HtmlTextWriterAttribute.Name, $"captcha");
        //            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
        //            writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-control");
        //            writer.RenderBeginTag(HtmlTextWriterTag.Input);
        //            writer.RenderEndTag(); // end Input
        //        }
        //        return new MvcHtmlString(stringWriter.ToString());
        //    }
        //}

        //private static MvcHtmlString CaptchaMathString<TModel>(this HtmlHelper<TModel> helper, string captchaCode)
        //{
        //    StringWriter stringWriter = new StringWriter();
        //    using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
        //    {
        //        writer.RenderBeginTag(HtmlTextWriterTag.Span);
        //        writer.RenderEndTag(); // end Img
        //    }
        //    return new MvcHtmlString(stringWriter.ToString());

        //}

        //private static MvcHtmlString CaptchaImage<TModel>(this HtmlHelper<TModel> helper, string captchaCode)
        //{
        //    int height = 30;
        //    int width = 100;
        //    Bitmap bmp = new Bitmap(width, height);
        //    RectangleF rectf = new RectangleF(10, 5, 0, 0);

        //    // generazione dell'immagine del captcha
        //    Graphics g = Graphics.FromImage(bmp);
        //    g.Clear(Color.White);
        //    g.SmoothingMode = SmoothingMode.AntiAlias;
        //    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //    g.DrawString(captchaCode, new Font("Thaoma", 12, FontStyle.Italic), Brushes.Green, rectf);
        //    g.DrawRectangle(new Pen(Color.Red), 1, 1, width - 2, height - 2);
        //    g.Flush();

        //    StringWriter stringWriter = new StringWriter();
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        bmp.Save(ms, ImageFormat.Jpeg);
        //        string imageBase64Data = Convert.ToBase64String(ms.ToArray());
        //        string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);

        //        g.Dispose();
        //        bmp.Dispose();

        //        using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
        //        {
        //            writer.AddAttribute(HtmlTextWriterAttribute.Id, $"captchaImage");
        //            writer.AddAttribute(HtmlTextWriterAttribute.Alt, "captcha");
        //            writer.AddAttribute(HtmlTextWriterAttribute.Class, "icon icon-captcha");
        //            writer.AddAttribute(HtmlTextWriterAttribute.Src, imageDataURL);
        //            writer.RenderBeginTag(HtmlTextWriterTag.Img);
        //            writer.RenderEndTag(); // end Img
        //        }
        //        return new MvcHtmlString(stringWriter.ToString());
        //    }
        //}

        //private static MvcHtmlString CaptchaHiddenField<TModel>(this HtmlHelper<TModel> helper, string captchaCode)
        //{
        //    string codeToEncode = $"{captchaCode}";
        //    string captchaEncriptedCode = $"CC_{RER.Tools.DesCryptor.EncryptText(captchaCode, (string)Microsoft.Win32.Registry.LocalMachine.OpenSubKey(RER.Tools.Configuration.RootRegKey).GetValue("Test"))}";
        //    StringWriter stringWriter = new StringWriter();
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
        //        {
        //            writer.AddAttribute(HtmlTextWriterAttribute.Id, $"controlCode");
        //            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
        //            writer.AddAttribute(HtmlTextWriterAttribute.Value, captchaEncriptedCode);
        //            writer.RenderBeginTag(HtmlTextWriterTag.Input);
        //            writer.RenderEndTag(); // end Input
        //        }
        //        return new MvcHtmlString(stringWriter.ToString());
        //    }
        //}

        //public static MvcHtmlString Captcha<TModel>(this HtmlHelper<TModel> helper, RER.Tools.MVC.Agid.Captcha.CaptchaType type = RER.Tools.MVC.Agid.Captcha.CaptchaType.Math)
        //{
        //    if (type == RER.Tools.MVC.Agid.Captcha.CaptchaType.Image)
        //        return ImageCaptcha(helper);
        //    else
        //        return MathCaptcha(helper);
        //}

        //public static MvcHtmlString ImageCaptcha<TModel>(this HtmlHelper<TModel> helper)
        //{
        //    string captchaCode = GetCaptchaImageCode();
        //    StringWriter stringWriter = new StringWriter();
        //    using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
        //    {

        //        writer.RenderBeginTag(HtmlTextWriterTag.Table);
        //        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        //        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        //        writer.Write(CaptchaLabel(helper));
        //        writer.Write(CaptchaTextBox(helper));
        //        writer.RenderEndTag();
        //        writer.RenderBeginTag(HtmlTextWriterTag.Td);


        //        writer.Write(CaptchaHiddenField(helper, captchaCode));
        //        writer.Write(CaptchaImage(helper, captchaCode));
        //        writer.RenderEndTag();
        //        writer.RenderEndTag();
        //        writer.RenderEndTag();




        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-group");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.Div);

        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.Div);

        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-prepend");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-text");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.Div);

        //        //writer.Write(CaptchaHiddenField(helper, captchaCode));
        //        //writer.Write(CaptchaImage(helper, captchaCode));
        //        //writer.WriteBreak();
        //        ////writer.RenderEndTag(); // end Div input-group-text
        //        ////writer.RenderEndTag(); // end Div input-group-prepend

        //        //writer.Write(CaptchaLabel(helper));
        //        //writer.Write(CaptchaTextBox(helper));

        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-append");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        ////writer.RenderEndTag(); // end Div input-group-append

        //        ////writer.RenderEndTag(); // end Div input-group
        //        ////writer.RenderEndTag(); // end Div form-group
        //    }
        //    return new MvcHtmlString(stringWriter.ToString());
        //}

        //public static MvcHtmlString MathCaptcha<TModel>(this HtmlHelper<TModel> helper)
        //{
        //    string captchaCode = GetCaptchaMathCode();
        //    StringWriter stringWriter = new StringWriter();
        //    using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
        //    {

        //        writer.RenderBeginTag(HtmlTextWriterTag.Table);
        //        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        //        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        //        writer.Write(CaptchaLabel(helper));
        //        writer.Write(CaptchaTextBox(helper));
        //        writer.RenderEndTag();
        //        writer.RenderBeginTag(HtmlTextWriterTag.Td);


        //        writer.Write(CaptchaHiddenField(helper, captchaCode));
        //        writer.Write(CaptchaImage(helper, captchaCode));
        //        writer.RenderEndTag();
        //        writer.RenderEndTag();
        //        writer.RenderEndTag();




        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-group");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.Div);

        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.Div);

        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-prepend");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-text");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.Div);

        //        //writer.Write(CaptchaHiddenField(helper, captchaCode));
        //        //writer.Write(CaptchaImage(helper, captchaCode));
        //        //writer.WriteBreak();
        //        ////writer.RenderEndTag(); // end Div input-group-text
        //        ////writer.RenderEndTag(); // end Div input-group-prepend

        //        //writer.Write(CaptchaLabel(helper));
        //        //writer.Write(CaptchaTextBox(helper));

        //        ////writer.AddAttribute(HtmlTextWriterAttribute.Class, "input-group-append");
        //        ////writer.RenderBeginTag(HtmlTextWriterTag.Div);
        //        ////writer.RenderEndTag(); // end Div input-group-append

        //        ////writer.RenderEndTag(); // end Div input-group
        //        ////writer.RenderEndTag(); // end Div form-group
        //    }
        //    return new MvcHtmlString(stringWriter.ToString());
        //}
    }
}
