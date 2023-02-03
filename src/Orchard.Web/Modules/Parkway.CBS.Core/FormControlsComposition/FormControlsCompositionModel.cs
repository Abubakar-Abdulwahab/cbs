namespace Parkway.CBS.Core.FormControlsComposition
{
    public class FormControlsCompositionModel
    {
        public string ClassAssembly { get; set; }

        public string ClassName { get; set; }

        public string ClassAssemblyAndName
        {
            get
            {
                return this.ClassAssembly + this.ClassName;
            }
        }
    }
}