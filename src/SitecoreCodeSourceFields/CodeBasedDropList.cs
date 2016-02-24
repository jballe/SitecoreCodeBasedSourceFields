using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Globalization;

namespace SitecoreCodeSourceFields
{
    public sealed class CodeBasedDropList : Sitecore.Web.UI.HtmlControls.Control
    {
        private bool _hasPostData;

        [UsedImplicitly]
        public string FieldName { get; set; }

        [UsedImplicitly]
        public string Source { get; set; }

        public CodeBasedDropList()
        {
            FieldName = string.Empty;
            Source = string.Empty;
            Class = "scContentControl scCombobox";
            Activation = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (_hasPostData)
                return;
            LoadPostData(string.Empty);
        }

        protected override void OnPreRender(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnPreRender(e);

            // Why this?
            ServerProperties["Value"] = ServerProperties["Value"];
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");

            IEnumerable<KeyValuePair<string, string>> values;
            string errorMessage;
            if (!DataLoader.SafeGetKeyValue(Source, out values, out errorMessage))
            {
                output.Write(errorMessage);
                return;
            }

            output.Write("<select" + GetControlAttributes() + ">");
            var items = GetListItems(values).ToList();
            foreach (var item in items)
            {
                output.Write("<option value=\"" + item.Value + "\""
                    + (item.Selected ? " selected=\"selected\"" : string.Empty) + ">"
                    + item.Text + "</option>");
            }

            var notAnySelected = !items.Any(x => x.Selected);
            var notifyNotSelected = notAnySelected && !string.IsNullOrEmpty(Value);
            if (notifyNotSelected)
            {
                output.Write("<optgroup label=\"Baah " + Translate.Text("Value not in the selection list.") + "\">");
                output.Write("<option value=\"" + Value + "\" selected=\"selected\">" + Value + "</option>");
                output.Write("</optgroup>");
            }

            output.Write("</select>");
            if (notifyNotSelected)
            {
                output.Write("<div style=\"color:#999999;padding:2px 0px 0px 0px\">{0}</div>", Translate.Text("The field contains a value that is not in the selection list."));
            }
        }

        private IEnumerable<ListItem> GetListItems(IEnumerable<KeyValuePair<string, string>> values)
        {
            yield return new ListItem
            {
                Selected = string.IsNullOrEmpty(Value),
                Value = string.Empty,
                Text = string.Empty
            };

            var found = false;
            foreach (var item in values)
            {
                var value = item.Key;
                var text = item.Value;
                var selected = Value == value;
                found = found || selected;

                yield return new ListItem
                {
                    Text = text,
                    Value = value,
                    Selected = selected
                };
            }

            if (!found)
            {
                yield return new ListItem
                {
                    Text = "Not in list: " + Value,
                    Value = Value,
                    Selected = true
                };
            }
        }

        protected override bool LoadPostData(string value)
        {
            _hasPostData = true;
            if (value == null)
                return false;
            if (GetViewStateString("Value") != value)
                SetModified();
            SetViewStateString("Value", value);
            return true;
        }

        private static void SetModified()
        {
            Sitecore.Context.ClientPage.Modified = true;
        }

    }
}
