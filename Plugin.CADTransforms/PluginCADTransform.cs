using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NthStudio.Plugins;
using Plugin.CADTransforms.Controls;


namespace Plugin.CADTransforms
{
    [Plugin("NthStudio CAD Core",
     "NthStudio CAD Transformation Library with Convex hull Constructive Solid Geometry",
     "Stylianos Polychroniadis",
     "1.1.0",
     "17-05-2016")]
    public class PluginCADTransform : NthStudio.IoC.BasePluginControl
    {
        TransformationSpace     _space;             // todo
        TranslateRotateScale    _transform;         // todo
        ConstructivePolygon     _solid;             // todo

        public override void InitializeComponent()
        {
            _space              = new TransformationSpace();
            _transform          = new TranslateRotateScale();
            _solid              = new ConstructivePolygon();

            this.Widgets.Add(_space);
            this.Widgets.Add(_transform);
            this.Widgets.Add(_solid);

            base.InitializeComponent();
        }
    }
}
