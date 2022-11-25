using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using WwwDocs;
using WwwDocs.Shared;
using System.Text;

namespace WwwDocs.Pages
{
    public partial class Index
    {
        public string InitialData { get; set; }

        public string ProcessedData { get; set; }

        public void Process()
        {
            if (!string.IsNullOrEmpty(InitialData))
            {
                ProcessedData = this.GetProcessedData(this.InitialData);
            }
        }

        private string GetProcessedData(string data)
        {
            return generatorService.GetData(data);
        }
    }
}