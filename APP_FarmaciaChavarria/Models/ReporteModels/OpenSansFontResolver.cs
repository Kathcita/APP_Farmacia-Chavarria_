using PdfSharpCore.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.Models.ReporteModels
{

    public class OpenSansFontResolver : IFontResolver
    {

        private static readonly byte[] FontData;

        public string DefaultFontName => "OpenSans#";

        static OpenSansFontResolver()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("APP_FarmaciaChavarria.Resources.Fonts.OpenSans-Regular1.ttf"); // Cambia por tu ruta real
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            FontData = ms.ToArray();
        }
        public byte[] GetFont(string faceName) => FontData;

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            return new FontResolverInfo("OpenSans#");
        }
    }
}
