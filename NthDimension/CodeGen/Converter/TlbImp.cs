namespace NthDimension.CodeGen.Converter
{
    // ToDo Rename to TypeLibraryImplementation

    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Collections;
    using System.IO;
    using Microsoft.Win32;
    using Project;


    internal class ConversionEventHandler : ITypeLibImporterNotifySink
    {
        private TlbImp Parent = null;
        public ConversionEventHandler(TlbImp parent)
        {
            Parent = parent;
        }
        public void ReportEvent(ImporterEventKind eventKind, int eventCode, string eventMsg)
        {
            Parent.InvokeReportEvent(new ReportEventEventArgs(eventKind, eventCode, eventMsg));
        }

        public Assembly ResolveRef(object typeLib)
        {
            ITypeLib tLib = typeLib as ITypeLib;
            string tLibname;
            string outputFolder = "";
            string interopFileName;
            string asmPath;
            try
            {
                tLibname = Marshal.GetTypeLibName(tLib);
                IntPtr ppTlibAttri = IntPtr.Zero;
                tLib.GetLibAttr(out ppTlibAttri);
                System.Runtime.InteropServices.ComTypes.TYPELIBATTR tLibAttri = (System.Runtime.InteropServices.ComTypes.TYPELIBATTR)Marshal.PtrToStructure(ppTlibAttri, typeof(System.Runtime.InteropServices.ComTypes.TYPELIBATTR));
                string guid = tLibAttri.guid.ToString();
                RegistryKey typeLibsKey = Registry.ClassesRoot.OpenSubKey("TypeLib\\{" + guid + "}");
                TypeLibrary typeLibrary = TypeLibrary.Create(typeLibsKey);
                Converter.TlbImp importer = new Converter.TlbImp(Parent.References);
                outputFolder = Path.GetDirectoryName(this.Parent.AsmPath);
                interopFileName = Path.Combine(outputFolder, String.Concat("Interop.", typeLibrary.Name, ".dll"));

                asmPath = interopFileName;
                //asm.Save(Path.GetFileName(asmPath));
                //Call ResolveRefEventArgs to notify extra assembly imported as depenedencies
                ResolveRefEventArgs t = new ResolveRefEventArgs("Reference Interop '" + Path.GetFileName(asmPath) + "' created succesfully.");
                Parent.InvokeResolveRef(t);
                Parent.References.Add(new ComReferenceProjectItem(NthDimension.Rendering.ApplicationBase.Instance.Compiler.GetDotNetProject(), typeLibrary));
                return importer.Import(interopFileName, typeLibrary.Path, typeLibrary.Name, this.Parent);

            }
            catch (Exception Ex)
            {
                ResolveRefEventArgs t = new ResolveRefEventArgs(Ex.Message);
                Parent.InvokeResolveRef(t);
                return null;
            }
            finally
            {
                Marshal.ReleaseComObject(tLib);
            }
        }
    }

    public class ReportEventEventArgs : EventArgs
    {
        public ImporterEventKind EventKind;
        public int EventCode;
        public string EventMsg;
        public ReportEventEventArgs(ImporterEventKind eventKind, int eventCode, string eventMsg)
        {
            EventKind = eventKind;
            EventCode = eventCode;
            EventMsg = eventMsg;
        }
    }

    public class ResolveRefEventArgs : EventArgs
    {

        public string Message = "";

        public ResolveRefEventArgs(string message)
        {
            Message = message;
        }
    }

    public class TlbImp
    {
        public enum REGKIND
        {
            REGKIND_DEFAULT = 0,
            REGKIND_REGISTER,
            REGKIND_NONE
        }

        private enum RegKind
        {
            RegKind_Default = 0,
            RegKind_Register = 1,
            RegKind_None = 2
        }

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void LoadTypeLibEx(String strTypeLibName, RegKind regKind, out ITypeLib typeLib);

        public event EventHandler<ReportEventEventArgs> ReportEvent;
        public event EventHandler<ResolveRefEventArgs> ResolveRef;
        public string AsmPath = "";
        public ArrayList References;
        protected virtual void OnReportEvent(ReportEventEventArgs e)
        {
            if (ReportEvent != null)
            {
                ReportEvent(this, e);
            }

        }
        protected virtual void OnResolveRef(ResolveRefEventArgs e)
        {
            if (ResolveRef != null)
            {
                ResolveRef(this, e);
            }
        }

        public void InvokeResolveRef(ResolveRefEventArgs e)
        {
            OnResolveRef(e);
        }

        public void InvokeReportEvent(ReportEventEventArgs e)
        {
            OnReportEvent(e);
        }

        public TlbImp(ArrayList references)
        {
            References = references;
        }
        public Assembly Import(string InteropFileName, string path, string name)
        {
            return Import(InteropFileName, path, name, null);
        }

        public Assembly Import(string InteropFileName, string path, string name, TlbImp parent)
        {
            ITypeLib typeLib;
            AsmPath = Path.GetDirectoryName(InteropFileName);
            LoadTypeLibEx(path, RegKind.RegKind_None, out typeLib);

            if (typeLib == null)
            {
                return null;
            }
            TypeLibConverter converter = new TypeLibConverter();
            ConversionEventHandler eventHandler = new ConversionEventHandler(parent == null ? this : parent);
            AssemblyBuilder asm =
                converter.ConvertTypeLibToAssembly(typeLib, Path.GetFileName(InteropFileName), TypeLibImporterFlags.None, eventHandler, null, null, name, null);
            string outputFolder = Path.GetDirectoryName(InteropFileName);
            string interopFName = Path.Combine(outputFolder, String.Concat("Interop.", name, ".dll"));
            asm.Save(Path.GetFileName(interopFName));
            Marshal.ReleaseComObject(typeLib);
            return asm as Assembly;


        }
    }
}
