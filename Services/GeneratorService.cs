using System.Text;

namespace WwwDocs.Services
{
    public class GeneratorService
    {
        public string GetData(string data)
        {
            var sb = new StringBuilder();
            var splittedInitialData = data.Split(new string[] { "\n" }, StringSplitOptions.None);

            var className = string.Empty;
            var properties = new List<string>();

            var attributesCache = new List<string>();

            foreach (var item in splittedInitialData)
            {
                var cleanSplit = item.Split(new string[] { " " }, StringSplitOptions.None).Where(x => x.Length > 0).ToArray();
                var cleanItem = string.Join(' ', cleanSplit);

                // If it's an attribute
                if (cleanItem.Trim().StartsWith("[") && cleanItem.Trim().EndsWith("]"))
                {
                    attributesCache.Add(cleanItem);
                    continue;
                }

                // If it's using
                if (cleanItem.StartsWith("using "))
                {
                    sb.AppendLine(cleanItem);
                    continue;
                }

                // If it's namespace
                if (cleanItem.Contains(" namespace "))
                {
                    //sb.AppendLine("/// <summary>");
                    //sb.AppendLine("/// ");
                    //sb.AppendLine("/// <summary>");
                }

                // If its classname
                if (cleanItem.Contains(" class "))
                {
                    var classSplit = cleanItem.Split(new string[] { " " }, StringSplitOptions.None);

                    if (cleanItem.Contains(":"))
                    {
                        classSplit = cleanItem.Split(new string[] { " : " }, StringSplitOptions.None)[0].Trim()
                            .Split(new string[] { " " }, StringSplitOptions.None);
                    }

                    var classIndex = classSplit.Count() - 1;
                    className = classSplit[classIndex];

                    sb.AppendLine("/// <summary>");
                    sb.AppendLine($"/// {className} class");
                    sb.AppendLine("/// </summary>");

                    foreach (var attribute in attributesCache)
                    {
                        sb.AppendLine(attribute);
                    }

                    attributesCache.Clear();
                }

                // If it's property

                if ((cleanItem.Contains("private") || cleanItem.Contains("public") || cleanItem.Contains("{ get; set; }")) && cleanItem.EndsWith($";"))
                {
                    // Get property name
                    var propertySplit = cleanItem.Split(new string[] { " " }, StringSplitOptions.None).ToList();

                    // Contains definition? =
                    if (propertySplit.Contains("="))
                    {
                        propertySplit = cleanItem.Split(new string[] { "=" }, StringSplitOptions.None)[0].Split(new string[] { " " }, StringSplitOptions.None).Where(x => x.Length > 0).ToList();
                    }

                    // Remove last, take last one
                    var propertyName = $"{propertySplit.Last().Replace(";", "")}";

                    sb.AppendLine("/// <summary>");

                    if (item.Contains("{ get; set; }"))
                    {
                        sb.AppendLine($"/// Gets or sets the <see cref=\"{propertyName}\"/>");
                    }
                    else
                    {
                        sb.AppendLine($"/// <see cref=\"{propertyName}\"/> object");
                    }

                    sb.AppendLine("/// </summary>");

                    foreach (var attribute in attributesCache)
                    {
                        sb.AppendLine(attribute);
                    }

                    attributesCache.Clear();
                }


                // If it's constructor
                if (cleanItem.Contains($"public {className}(") || item.Contains($"public {className} ("))
                {
                    // Remove end constructor
                    var cleanUitem = cleanItem.Replace(")", "");

                    // Get parameters
                    var parametersStr = cleanUitem.Split(new string[] { "(" }, StringSplitOptions.None)[1];
                    var parameters = parametersStr.Split(new string[] { "," }, StringSplitOptions.None);

                    sb.AppendLine("/// <summary>");
                    sb.AppendLine($"/// Constructor for <see cref=\"{className}\"/>");
                    sb.AppendLine("/// </summary>");

                    foreach (var param in parameters)
                    {
                        var paramClean = param.Trim();
                        var paramSplit = paramClean.Split(new string[] { " " }, StringSplitOptions.None).Where(x => x.Length > 0).ToList();

                        sb.AppendLine($"/// <param cref=\"{paramSplit[0]}\" name=\"{paramSplit[1]}\">Parameter for <see cref=\"{paramSplit[1]}\"/></param>");
                    }
                    //sb.AppendLine($"/// <returns>An object of type <see cref=\"{className}\"></returns>");

                    foreach (var attribute in attributesCache)
                    {
                        sb.AppendLine(attribute);
                    }

                    attributesCache.Clear();
                }

                // If it's method
                if ((cleanItem.StartsWith($"private") || cleanItem.StartsWith($"public")) && (cleanItem.Contains("(") && cleanItem.Contains(")")) && !cleanItem.StartsWith($"public {className}") && !cleanItem.EndsWith($";"))
                {
                    // Initial split
                    var split = item.Split(new string[] { " " }, StringSplitOptions.None).ToList();

                    // Method name
                    var methodName = cleanItem.Split(new string[] { "(" }, StringSplitOptions.None)[0]
                        .Split(new string[] { " " }, StringSplitOptions.None).Last();

                    // Method return
                    var methodReturn = cleanItem.Split(new string[] { $"{methodName}" }, StringSplitOptions.None)[0].Trim()
                        .Split(new string[] { " " }, StringSplitOptions.None).Last();

                    // Get parameters
                    var parametersStr = cleanItem.Split(new string[] { "(" }, StringSplitOptions.None)[1].Replace(")", "").Replace("[Required]", ""); ;
                    var parameters = parametersStr.Split(new string[] { ", " }, StringSplitOptions.None).Where(x => x.Length > 0).ToList();

                    // Check for dictionaries, tuples, etc
                    var finalParameters = new List<string>();

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        string? par = parameters[i];

                        if (i > 0)
                        {
                            if (finalParameters[i - 1].Contains("Dictionary"))
                            {
                                // add to previous
                                finalParameters[i - 1] = finalParameters[i - 1] + ", " + par;
                            }
                            else
                            {
                                finalParameters.Add(par);
                            }
                        }
                        else
                        {
                            finalParameters.Add(par);
                        }
                    }

                    sb.AppendLine("/// <summary>");
                    sb.AppendLine($"/// {methodName}");
                    sb.AppendLine("/// </summary>");

                    foreach (var param in finalParameters)
                    {
                        var paramClean = param.Trim();
                        var paramSplit = paramClean.Split(new string[] { " " }, StringSplitOptions.None).Where(x => x.Length > 0).ToList();

                        if (!paramClean.Contains("Dictionary"))
                        {
                            // TODO
                            sb.AppendLine($"/// <param cref=\"{paramSplit[0].Replace("<", "{").Replace(">", "}")}\" name=\"{paramSplit[1]}\">Parameter for <see cref=\"{paramSplit[1]}\"/></param>");
                        }
                    }
                    if (methodReturn != "void")
                    {
                        sb.AppendLine($"/// <returns>An object of type <see cref=\"{methodReturn.Replace("<", "{").Replace(">", "}")}\"/></returns>");
                    }

                    foreach (var attribute in attributesCache)
                    {
                        sb.AppendLine(attribute);
                    }

                    attributesCache.Clear();
                }

                sb.AppendLine(cleanItem);
            }

            // Return
            return sb.ToString();
        }
    }
}
