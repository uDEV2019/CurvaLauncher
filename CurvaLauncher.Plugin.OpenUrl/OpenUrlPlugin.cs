﻿using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Threading;
using CurvaLauncher.Utilities;
using CurvaLauncher.Data;

namespace CurvaLauncher.Plugin.OpenUrl
{
    public class OpenUrlPlugin : ISyncPlugin
    {
        public static Regex UrlRegex { get; } 
            = new Regex(@"(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?");

        public static string IconSvg { get; }
            = "<svg t=\"1695798632900\" class=\"icon\" viewBox=\"0 0 1024 1024\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" p-id=\"26186\" width=\"128\" height=\"128\"><path d=\"M952.888889 281.6V910.222222c0 62.862222-50.915556 113.777778-113.777778 113.777778H156.444444c-62.862222 0-113.777778-50.915556-113.777777-113.777778V113.777778c0-62.862222 50.915556-113.777778 113.777777-113.777778h514.844445L952.888889 281.6z\" fill=\"#58C5C2\" p-id=\"26187\"></path><path d=\"M511.146667 294.656l0.711111 0.142222a246.044444 246.044444 0 0 1 0 491.292445 17.92 17.92 0 0 1-5.688889 0.284444h-0.995556l-7.395555 0.113778a246.044444 246.044444 0 1 1 8.391111-491.946667 17.237333 17.237333 0 0 1 4.977778 0.113778z m-98.986667 355.271111l-92.501333 0.028445a209.180444 209.180444 0 0 0 144.896 96.938666 334.961778 334.961778 0 0 1-52.394667-96.938666z m263.736889 0.028445H592.497778a337.92 337.92 0 0 1-50.062222 94.776888 209.351111 209.351111 0 0 0 133.461333-94.776888z m-122.737778 0h-101.603555a298.069333 298.069333 0 0 0 51.2 85.248c21.902222-26.026667 38.684444-54.897778 50.403555-85.248z m-155.534222-159.288889h-102.968889A209.578667 209.578667 0 0 0 288.711111 540.444444c0 25.486222 4.551111 49.92 12.913778 72.533334h100.181333a338.773333 338.773333 0 0 1-4.181333-122.311111z m171.832889 0.028444h-134.4a301.511111 301.511111 0 0 0 4.721778 122.282667h125.013333c9.813333-40.078222 11.377778-81.720889 4.664889-122.282667z m131.413333 0h-93.980444a342.186667 342.186667 0 0 1-4.152889 122.282667h91.192889A208.64 208.64 0 0 0 706.844444 540.444444c0-17.152-2.076444-33.820444-5.973333-49.777777z m-236.288-156.728889l-0.711111 0.142222A209.351111 209.351111 0 0 0 307.484444 453.688889h97.735112a334.705778 334.705778 0 0 1 59.335111-119.694222z m38.200889 11.719111l-3.868444 4.579556a298.012444 298.012444 0 0 0-55.239112 103.452444h117.248a301.397333 301.397333 0 0 0-58.140444-108.032z m39.651556-9.528889l4.295111 5.831112a338.062222 338.062222 0 0 1 52.622222 111.701333h88.689778a209.464889 209.464889 0 0 0-145.607111-117.532445z\" fill=\"#FFFFFF\" p-id=\"26188\"></path><path d=\"M676.664889 167.822222V0l281.6 281.6h-167.822222c-62.862222 0-113.777778-50.915556-113.777778-113.777778\" fill=\"#2B9592\" p-id=\"26189\"></path></svg>";

        public ImageSource Icon => ImageUtils.CreateFromSvg(IconSvg);

        public string Name => "Open URL";

        public string Description => "Use web browser to open URL";

        public void Init()
        {
            // do nothing
        }

        public IEnumerable<QueryResult> Query(CurvaLauncherContext context, string query)
        {
            if (!UrlRegex.IsMatch(query))
                yield break;
            if (!Uri.TryCreate(query, UriKind.Absolute, out var uri))
                yield break;
            if (!uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) &&
                !uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                yield break;

            yield return new OpenUrlQueryResult(context, uri);
        }
    }
}