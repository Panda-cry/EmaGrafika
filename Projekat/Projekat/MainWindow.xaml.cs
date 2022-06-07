using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using WpfApp1.Model;
using Point = System.Windows.Point;

namespace Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Objects
    {
        GeometryModel3D model;
        string tooltip;
      

        public Objects(GeometryModel3D models, string toolTip)
        {
            this.model = models;
            Tooltip = toolTip;
        }

        public GeometryModel3D Model { get => model; set => model = value; }
        public string Tooltip { get => tooltip; set => tooltip = value; }
    }
    public class CatchEntity
    {
        GeometryModel3D model;
        long start;
        long end;

        public CatchEntity(GeometryModel3D model, long start, long end)
        {
            this.Model = model;
            this.Start = start;
            this.End = end;
        }

        public GeometryModel3D Model { get => model; set => model = value; }
        public long Start { get => start; set => start = value; }
        public long End { get => end; set => end = value; }
    }
    public class Lines
    {
        GeometryModel3D model;
        double r;
        string material;

        public Lines(GeometryModel3D model, double r, string material)
        {
            this.Model = model;
            this.R = r;
            this.Material = material;
           
        }

        public string Material { get => material; set => material = value; }
        public GeometryModel3D Model { get => model; set => model = value; }
        public double R { get => r; set => r = value; }
    }
    public class FindMe
    {
        public int X { get; set; }
        public int Y { get; set; }
        public long Id { get; set; }

        public FindMe(int x, int y, long id)
        {
            X = x;
            Y = y;
            this.Id = id;
        }
    }
    public partial class MainWindow : Window
    {
        public double noviX, noviY;
        private System.Windows.Point start = new System.Windows.Point();
        private System.Windows.Point diffOffset = new System.Windows.Point();
        private int zoomMax = 20;
        private int zoomCurent = 1;

        Dictionary<long, GeometryModel3D> nodess = new Dictionary<long, GeometryModel3D>();
        Dictionary<long, GeometryModel3D> subs = new Dictionary<long, GeometryModel3D>();
        Dictionary<long, GeometryModel3D> switchs = new Dictionary<long, GeometryModel3D>();
        Dictionary<long, int> counter = new Dictionary<long, int>();

        private int[,] array = new int[501, 501];
        List<Objects> objekti = new List<Objects>();
        Dictionary<GeometryModel3D, string> changeSwitches = new Dictionary<GeometryModel3D, string>();
        List<Lines> lineChanges = new List<Lines>();
        List<FindMe> findMe = new List<FindMe>();
        Dictionary<long, GeometryModel3D> findModelsById = new Dictionary<long, GeometryModel3D>();
        private Point startPoint = new Point();
        bool changeSwitch = false;
        bool changeLine = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            XmlNodeList nodeList;

            

            SubstationEntity sub = new SubstationEntity();

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            foreach (XmlNode node in nodeList)
            {
                sub.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                sub.Name = node.SelectSingleNode("Name").InnerText;
                sub.X = double.Parse(node.SelectSingleNode("X").InnerText.Replace('.',','));
                sub.Y = double.Parse(node.SelectSingleNode("Y").InnerText.Replace('.',','));

                ToLatLon(sub.X, sub.Y, 34, out noviX, out noviY);

                if (noviX >= 45.2325 && noviX <= 45.277031 && noviY >= 19.793909 && noviY <= 19.894459)
                {

                    int x = 500 -(int)getX(noviX);
                    int y = 500 -(int)getY(noviY);

                    int calcX = x / 5;
                    int calcY = y / 5;

                    AddCube(calcX * 5, calcY *5, array[calcX * 5, calcY * 5] * 3, 128, 0, 0,string.Format("id : {0}, name : {1} type: substation",sub.Id,sub.Name),false,"sub",sub.Id);
                    findMe.Add(new FindMe(calcX * 5, calcY * 5, sub.Id));
                    array[calcX * 5, calcY * 5] += 1;


                    //substations.Add(new SubstationEntity()
                    //{
                    //    Name = sub.Name,
                    //    Id = sub.Id,
                    //    X = getX(noviX),
                    //    Y = getY(noviY)
                    //});
                }

            }

          

            NodeEntity nodeobj = new NodeEntity();

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            foreach (XmlNode node in nodeList)
            {
                nodeobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nodeobj.Name = node.SelectSingleNode("Name").InnerText;
                nodeobj.X = double.Parse(node.SelectSingleNode("X").InnerText.Replace('.', ','));
                nodeobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText.Replace('.', ','));

                ToLatLon(nodeobj.X, nodeobj.Y, 34, out noviX, out noviY);

                if (noviX >= 45.2325 && noviX <= 45.277031 && noviY >= 19.793909 && noviY <= 19.894459)
                {
                    int x = 500 - (int)getX(noviX);
                    int y = 500 - (int)getY(noviY);

                    int calcX = x / 5;
                    int calcY = y / 5;

                    AddCube(calcX * 5, calcY * 5, array[calcX * 5, calcY * 5] * 3, 0, 0, 128,string.Format("id : {0}, name : {1} type: node", nodeobj.Id, nodeobj.Name),false,"node",nodeobj.Id);
                    findMe.Add(new FindMe(calcX * 5, calcY * 5, nodeobj.Id));
                    array[calcX * 5, calcY * 5] += 1;

                    //nodes.Add(new NodeEntity()
                    //{
                    //    Name = nodeobj.Name,
                    //    Id = nodeobj.Id,
                    //    X = getX(noviX),
                    //    Y = getY(noviY)
                    //});
                }

            }

            

            SwitchEntity switchobj = new SwitchEntity();

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in nodeList)
            {
                switchobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                switchobj.Name = node.SelectSingleNode("Name").InnerText;
                switchobj.X = double.Parse(node.SelectSingleNode("X").InnerText.Replace('.', ','));
                switchobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText.Replace('.', ','));
                switchobj.Status = node.SelectSingleNode("Status").InnerText;

                ToLatLon(switchobj.X, switchobj.Y, 34, out noviX, out noviY);

                if (noviX >= 45.2325 && noviX <= 45.277031 && noviY >= 19.793909 && noviY <= 19.894459)
                {
                    int x = 500 - (int)getX(noviX);
                    int y = 500 - (int)getY(noviY);

                    int calcX = x / 5;
                    int calcY = y / 5;

                    AddCube(calcX * 5, calcY * 5, array[calcX * 5, calcY * 5] * 3, 50, 205, 50, string.Format("id : {0}, name : {1} type: switch", switchobj.Id, switchobj.Name),true,switchobj.Status,switchobj.Id);
                    findMe.Add(new FindMe(calcX * 5, calcY * 5, switchobj.Id));
                    array[calcX * 5, calcY * 5] += 1;

                    //switches.Add(new SwitchEntity()
                    //{
                    //    Name = switchobj.Name,
                    //    Id = switchobj.Id,
                    //    Status = switchobj.Status,
                    //    X = getX(noviX),
                    //    Y = getY(noviY)
                    //});
                }
            }


            int counter = 0;

            List<Point> points = new List<Point>();

            LineEntity l = new LineEntity();

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            foreach (XmlNode node in nodeList)
            {
                l.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                l.Name = node.SelectSingleNode("Name").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                {
                    l.IsUnderground = true;
                }
                else
                {
                    l.IsUnderground = false;
                }
                l.R = float.Parse(node.SelectSingleNode("R").InnerText.Replace('.', ','));
                l.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                l.LineType = node.SelectSingleNode("LineType").InnerText;
                l.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
                l.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText.Replace('.', ','));
                l.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText.Replace('.', ','));
                bool first = false;
                foreach (XmlNode pointNode in node.ChildNodes[9].ChildNodes) // 9 posto je Vertices 9. node u jednom line objektu
                {
                    System.Windows.Point p = new System.Windows.Point();

                    p.X = double.Parse(pointNode.SelectSingleNode("X").InnerText.Replace('.',',')) ;
                    p.Y = double.Parse(pointNode.SelectSingleNode("Y").InnerText.Replace('.', ','));

                    ToLatLon(p.X, p.Y, 34, out noviX, out noviY);
                    if (noviX >= 45.2325 && noviX <= 45.277031 && noviY >= 19.793909 && noviY <= 19.894459)
                    {
                        int x = 500 - (int)getX(noviX);
                        int y = 500 - (int)getY(noviY);

                        x /= 5;
                        x *= 5;
                        y /= 5;
                        y *= 5;
                        points.Add(new Point(x,y));
                        
                        
                    }
                }
                       
                AddLine(l, points);

          


                points.Clear();
            }
     

        }

        List<CatchEntity> catchEntities = new List<CatchEntity>();


         private void AddLine(LineEntity entity,List<Point> points)
        {
            GeometryModel3D model = new GeometryModel3D();
            MeshGeometry3D meshModel = new MeshGeometry3D();

            if (!findModelsById.ContainsKey(entity.FirstEnd))
                return;
            if (!findModelsById.ContainsKey(entity.SecondEnd))
                return;


            int xStart = 0;
            int yStart = 0;
            int xEnd = 0;
            int yEnd = 0;

            foreach (var item in findMe)
            {
                if (item.Id.Equals(entity.FirstEnd))
                {
                    xStart = item.X;
                    yStart = item.Y;
                }

                if (item.Id.Equals(entity.SecondEnd))
                {
                    xEnd = item.X;
                    yEnd = item.Y;
                }
            }

            if (xStart != 0 && yStart != 0)
            {
                points.RemoveAt(0);
                points.Insert(0, new Point(xStart, yStart));
            }

            else
                return;

            if (xEnd != 0 && yEnd != 0)
            {
                points.RemoveAt(points.Count - 1);
                points.Add(new Point(xEnd, yEnd));
            }

            else
                return;

            if (counter.ContainsKey(entity.FirstEnd))
            {
                counter[entity.FirstEnd] += 1;
            }
            else
            {
                counter.Add(entity.FirstEnd, 0);
            }
            if (counter.ContainsKey(entity.SecondEnd))
            {
                counter[entity.SecondEnd] += 1;
            }
            else
            {
                counter.Add(entity.SecondEnd, 0);
            }
            


            for (int i = 0; i < points.Count; i++)
            {

                int X = (int)points[i].X;
                int Y = (int)points[i].Y;
                Point3D points1 = new Point3D(X + 1, 1, Y + 1);
                Point3D points2 = new Point3D(X + 2, 1, Y + 1);
                Point3D points3 = new Point3D(X + 1, 2, Y + 2);
                Point3D points4 = new Point3D(X + 2, 2, Y + 2);

                meshModel.Positions.Add(points1);
                meshModel.Positions.Add(points2);
                meshModel.Positions.Add(points3);
                meshModel.Positions.Add(points4);




            }
            int tacka0 = 0;
            int tacka1 = 1;
            int tacka2 = 2;
            int tacka3 = 3;
            int tacka4 = 4;
            int tacka5 = 5;
            int tacka6 = 6;
            int tacka7 = 7;

            for (int i = 0; i < points.Count - 1; i++)
            {
                //gore Trouglovi
                meshModel.TriangleIndices.Add(tacka2 + (i * 4));
                meshModel.TriangleIndices.Add(tacka3 + (i * 4));
                meshModel.TriangleIndices.Add(tacka7 + (i * 4));
                meshModel.TriangleIndices.Add(tacka2 + (i * 4));
                meshModel.TriangleIndices.Add(tacka7 + (i * 4));
                meshModel.TriangleIndices.Add(tacka6 + (i * 4));

                //dole Trouglovi
                meshModel.TriangleIndices.Add(tacka0 + (i * 4));
                meshModel.TriangleIndices.Add(tacka1 + (i * 4));
                meshModel.TriangleIndices.Add(tacka5 + (i * 4));
                meshModel.TriangleIndices.Add(tacka0 + (i * 4));
                meshModel.TriangleIndices.Add(tacka5 + (i * 4));
                meshModel.TriangleIndices.Add(tacka4 + (i * 4));

                //levo Trouglovi
                meshModel.TriangleIndices.Add(tacka3 + (i * 4));
                meshModel.TriangleIndices.Add(tacka5 + (i * 4));
                meshModel.TriangleIndices.Add(tacka7 + (i * 4));
                meshModel.TriangleIndices.Add(tacka1 + (i * 4));
                meshModel.TriangleIndices.Add(tacka5 + (i * 4));
                meshModel.TriangleIndices.Add(tacka3 + (i * 4));

                //desno Trouglovi
                meshModel.TriangleIndices.Add(tacka0 + (i * 4));
                meshModel.TriangleIndices.Add(tacka6 + (i * 4));
                meshModel.TriangleIndices.Add(tacka2 + (i * 4));
                meshModel.TriangleIndices.Add(tacka0 + (i * 4));
                meshModel.TriangleIndices.Add(tacka4 + (i * 4));
                meshModel.TriangleIndices.Add(tacka2 + (i * 4));

                //prvi Trougao
                meshModel.TriangleIndices.Add(tacka0 + (i * 4));
                meshModel.TriangleIndices.Add(tacka1 + (i * 4));
                meshModel.TriangleIndices.Add(tacka3 + (i * 4));
                meshModel.TriangleIndices.Add(tacka0 + (i * 4));
                meshModel.TriangleIndices.Add(tacka3 + (i * 4));
                meshModel.TriangleIndices.Add(tacka2 + (i * 4));
            }





            DiffuseMaterial material = new DiffuseMaterial();
            SolidColorBrush brush = new SolidColorBrush();
            switch (entity.ConductorMaterial)
            {
                case "Copper":
                    {
                        brush.Color = Color.FromRgb(184, 134, 11);
                    }
                    break;
                case "Steel":
                    {
                        brush.Color = Color.FromRgb(105, 105, 105);
                    }
                    break;
                case "Acsr":
                    {
                        brush.Color = Color.FromRgb(192, 192, 192);
                    }
                    break;
                default:
                    {
                        brush.Color = Color.FromRgb(221, 160, 221);
                    }
                    break;
            }


            material.Brush = brush;
            model.Material = material;
            model.Transform = Transforms;
            model.Geometry = meshModel;
            Map.Children.Add(model);
            lineChanges.Add(new Lines(model, double.Parse(entity.R.ToString()), entity.ConductorMaterial));
            catchEntities.Add(new CatchEntity(model, entity.FirstEnd, entity.SecondEnd));
        }
        private void AddCube(int X , int Y ,int Z , int R , int G , int B, string toolTip,bool flag,string status,long id)
        {
            GeometryModel3D model = new GeometryModel3D();

            MeshGeometry3D meshModel = new MeshGeometry3D();

            Point3D points =  new Point3D( X,Z,  Y);
            Point3D points1 = new Point3D( X + 3, Z,  Y);
            Point3D points2 = new Point3D( X, Z,  Y + 3);
            Point3D points3 = new Point3D( X + 3, Z,  Y + 3);
            Point3D points4 = new Point3D( X, Z +3,  Y);
            Point3D points5 = new Point3D( X + 3, Z + 3,  Y);
            Point3D points6 = new Point3D( X, Z +3,  Y + 3);
            Point3D points7 = new Point3D( X + 3,Z+ 3,  Y + 3);

            meshModel.Positions.Add(points);
            meshModel.Positions.Add(points1);
            meshModel.Positions.Add(points2);
            meshModel.Positions.Add(points3);
            meshModel.Positions.Add(points4);
            meshModel.Positions.Add(points5);
            meshModel.Positions.Add(points6);
            meshModel.Positions.Add(points7);

            meshModel.TriangleIndices = new Int32Collection() { 2, 3, 7, 2, 7, 6, 2, 6, 4, 2, 4, 0, 2, 3, 1, 2, 1, 0, 1, 5, 7, 1, 7, 3, 1, 0, 4, 1, 4, 5, 5, 4, 6, 5, 6, 7 };

            DiffuseMaterial material = new DiffuseMaterial();
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromRgb((byte)R,(byte)G,(byte)B);
            material.Brush = brush;

            model.Geometry = meshModel;
            model.Material = material;
            model.Transform = Transforms;

            Map.Children.Add(model);
            findModelsById.Add(id, model);
            objekti.Add(new Objects(model, toolTip));

            if (status.Equals("sub"))
                subs.Add(id, model);
            if (status.Equals("node"))
                nodess.Add(id, model);

            if (flag)
            {
                changeSwitches.Add(model, status);
                if (status.Equals("Open"))
                {
                    switchs.Add(id, model);
                }
            }
              
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!changeSwitch)
            {
                foreach (var item in changeSwitches)
                {
                    GeometryModel3D changeModel = (GeometryModel3D)item.Key;
                    if(item.Value == "Closed")
                    {
                        DiffuseMaterial material = new DiffuseMaterial();
                        SolidColorBrush brush = new SolidColorBrush();
                        brush.Color = Color.FromRgb((byte)139, (byte)0, (byte)0);
                        material.Brush = brush;
                        changeModel.Material = material;
                    }
                    else
                    {
                        DiffuseMaterial material = new DiffuseMaterial();
                        SolidColorBrush brush = new SolidColorBrush();
                        brush.Color = Color.FromRgb(0, 255, 0); 
                        material.Brush = brush;
                        changeModel.Material = material;
                    }
                }
                changeSwitch = true;
            }
            else
            {
                foreach (var item in changeSwitches)
                {
                    GeometryModel3D changeModel = (GeometryModel3D)item.Key;
                    DiffuseMaterial material = new DiffuseMaterial();
                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = Color.FromRgb((byte)50, (byte)205, (byte)50);
                    material.Brush = brush;
                    changeModel.Material = material;
                }
                changeSwitch = false;
            }

        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (!changeLine)
            {
                foreach (var item in lineChanges)
                {
                    GeometryModel3D changeModel = (GeometryModel3D)item.Model;
                    if (item.R < 1 )
                    {
                        DiffuseMaterial material = new DiffuseMaterial();
                        SolidColorBrush brush = new SolidColorBrush();
                        brush.Color = Color.FromRgb(178, 34, 34);
                        material.Brush = brush;
                        changeModel.Material = material;
                    }
                    else if(item.R >=1 && item.R <=2)
                    {
                        DiffuseMaterial material = new DiffuseMaterial();
                        SolidColorBrush brush = new SolidColorBrush();
                        brush.Color = Color.FromRgb(255, 165, 0);
                        material.Brush = brush;
                        changeModel.Material = material;
                    }
                    else if (item.R > 2)
                    {
                        DiffuseMaterial material = new DiffuseMaterial();
                        SolidColorBrush brush = new SolidColorBrush();
                        brush.Color = Color.FromRgb(255, 255, 0);
                        material.Brush = brush;
                        changeModel.Material = material;
                    }
                }
                changeLine = true;
            }
            else
            {
                foreach (var item in lineChanges)
                {
                    GeometryModel3D changeModel = (GeometryModel3D)item.Model;
                    DiffuseMaterial material = new DiffuseMaterial();
                    SolidColorBrush brush = new SolidColorBrush();
                    switch (item.Material)
                    {
                        case "Copper":
                            {
                                brush.Color = Color.FromRgb(184, 134, 11);
                            }
                            break;
                        case "Steel":
                            {
                                brush.Color = Color.FromRgb(105, 105, 105);
                            }
                            break;
                        case "Acsr":
                            {
                                brush.Color = Color.FromRgb(192, 192, 192);
                            }
                            break;
                        default:
                            {
                                brush.Color = Color.FromRgb(221, 160, 221);
                            }
                            break;
                    }
                    material.Brush = brush;
                    changeModel.Material = material;
                }
                changeLine = false;
            }
        }

        private void notActive_Click(object sender, RoutedEventArgs e)
        {
            if (notActive.IsChecked)
            {
                foreach (var item in catchEntities)
                {
                    if (switchs.ContainsKey(item.Start))
                    {
                        GeometryModel3D mm = item.Model;
                        DiffuseMaterial material = (DiffuseMaterial)mm.Material;
                        SolidColorBrush brush = (SolidColorBrush)material.Brush;
                        brush.Color = Color.FromArgb(0, brush.Color.R, brush.Color.G, brush.Color.B);
                        getBack.Add(mm);
                        mm = switchs[item.Start];

                        material = (DiffuseMaterial)mm.Material;
                        brush = (SolidColorBrush)material.Brush;
                        brush.Color = Color.FromArgb(0, brush.Color.R, brush.Color.G, brush.Color.B);
                        getBack.Add(mm);
                        if (nodess.ContainsKey(item.End))
                        {
                            mm = nodess[item.End];
                            material = (DiffuseMaterial)mm.Material;
                            brush = (SolidColorBrush)material.Brush;
                            brush.Color = Color.FromArgb(0, brush.Color.R, brush.Color.G, brush.Color.B);
                            getBack.Add(mm);
                        }
                        if (subs.ContainsKey(item.End))
                        {
                            mm = subs[item.End];
                            material = (DiffuseMaterial)mm.Material;
                            brush = (SolidColorBrush)material.Brush;
                            brush.Color = Color.FromArgb(0, brush.Color.R, brush.Color.G, brush.Color.B);
                            getBack.Add(mm);
                        }
                        if (switchs.ContainsKey(item.End))
                        {
                            mm = switchs[item.End];
                            material = (DiffuseMaterial)mm.Material;
                            brush = (SolidColorBrush)material.Brush;
                            brush.Color = Color.FromArgb(0, brush.Color.R, brush.Color.G, brush.Color.B);
                            getBack.Add(mm);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in getBack)
                {
                    GeometryModel3D mm = item;
                    DiffuseMaterial material = (DiffuseMaterial)mm.Material;
                    SolidColorBrush brush = (SolidColorBrush)material.Brush;
                    brush.Color = Color.FromRgb(brush.Color.R, brush.Color.G, brush.Color.B);
                }
                getBack.Clear();
            }

        }
        #region Hit test
        private ArrayList models = new ArrayList();
        private GeometryModel3D hitgeo;
        private DiffuseMaterial blue = new DiffuseMaterial();
        void mainViewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
            System.Windows.Point mouseposition = e.GetPosition(viewport1);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

            PointHitTestParameters pointparams =
                     new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams =
                     new RayHitTestParameters(testpoint3D, testdirection);

            //test for a result in the Viewport3D     
            hitgeo = null;
            VisualTreeHelper.HitTest(viewport1, null, HTResult, pointparams);
        }

        private HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {

            RayHitTestResult rayResult = rawresult as RayHitTestResult;
            bool hitted = false;
            if (rayResult != null)
            {
                foreach (var item in objekti)
                {
                    if(item.Model == rayResult.ModelHit)
                    {
                        Canvas.SetLeft(txtblkTip, 600 - rayResult.ModelHit.Bounds.X );
                        Canvas.SetBottom(txtblkTip,300 - rayResult.ModelHit.Bounds.Y );

                         txtblkTip.Text = item.Tooltip;
                        hitted = true;
                        break;
                    }
                }
                foreach (var item in catchEntities)
                {
                    if(item.Model == rayResult.ModelHit)
                    {
                        GeometryModel3D modelChange = findModelsById[item.Start];
                        GeometryModel3D modelChange2 = findModelsById[item.End];
                        DiffuseMaterial material = new DiffuseMaterial();
                        SolidColorBrush brush = new SolidColorBrush();
                        brush.Color = Color.FromRgb(0, 255, 255);
                        material.Brush = brush;
                        modelChange.Material = material;
                        modelChange2.Material = material;
                    }
                }
               
            }
            if(!hitted)
                txtblkTip.Text = "";

            return HitTestResultBehavior.Stop;
        }

    
        #endregion

        #region Zoom & pan
        private void viewport1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewport1.CaptureMouse();
            start = e.GetPosition(this);
            diffOffset.X = translacija.OffsetX;
            diffOffset.Y = translacija.OffsetY;
        }

        private void viewport1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewport1.ReleaseMouseCapture();
        }

        private void viewport1_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewport1.IsMouseCaptured)
            {
                Point end = e.GetPosition(this);
                double offsetX = end.X - start.X;
                double offsetY = end.Y - start.Y;
                double w = this.Width;
                double h = this.Height;
                double translateX = (offsetX * 30900) / w;
                double translateY = -(offsetY * 30900) / h;
                translacija.OffsetX = (diffOffset.Y + (translateY / (100 * skaliranje.ScaleX)))*-1;
                translacija.OffsetZ =  (diffOffset.X + (translateX / (100 * skaliranje.ScaleX)))*-1;
            }
            if (scrool)
            {
              
                int translate = (int)(startPoint.X - e.GetPosition(this).X);
                myHorizontalRotation.Angle = translate;

            }


        }

        private void viewport1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p = e.MouseDevice.GetPosition(this);
            double scaleX = 1;
            double scaleZ = 1;
            if (e.Delta > 0 && zoomCurent < zoomMax)
            {
                scaleX = skaliranje.ScaleX + 0.1;
                scaleZ = skaliranje.ScaleZ + 0.1;
                zoomCurent++;
                skaliranje.ScaleX = scaleX;
                skaliranje.ScaleZ = scaleZ;
            }
            else if (e.Delta <= 0 && zoomCurent > -zoomMax)
            {
                scaleX = skaliranje.ScaleX - 0.1;
                scaleZ = skaliranje.ScaleZ - 0.1;
                zoomCurent--;
                skaliranje.ScaleX = scaleX;
                skaliranje.ScaleZ = scaleZ;
            }
        }
        #endregion


        #region MappingFunctions
        private double getX(double x)
        {
            double maxLat = 45.277031;
            double minLat = 45.2325;
            var position = (x - minLat) / (maxLat - minLat);
            return Math.Floor(499 * position);
           
        }
        private double getY(double y)
        {
            double minLon = 19.793909;
            double maxLon = 19.894459;
            var position = (y - minLon) / (maxLon - minLon);
            return Math.Floor(499 * position);
        }

        bool scrool = false;
        private void viewport1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                scrool = true;
                startPoint = e.GetPosition(this);
            }
           
        }
        List<GeometryModel3D> getBack = new List<GeometryModel3D>();

        private void viewport1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (scrool)
            {
                scrool = false;
      

            }
        }


        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool pet = comboBox.SelectedItem.ToString().Contains("5+");
            bool tri = comboBox.SelectedItem.ToString().Contains("0-3");
            bool izmedju = comboBox.SelectedItem.ToString().Contains("3-5");

            if (pet)
            {
                foreach (var item in counter)
                {
                    if(item.Value >=5)
                    {
                        GeometryModel3D mm = findModelsById[item.Key];
                        DiffuseMaterial material = (DiffuseMaterial)mm.Material;
                        SolidColorBrush brush = (SolidColorBrush)material.Brush;
                        brush.Color = Color.FromArgb(0, brush.Color.R, brush.Color.G, brush.Color.B);
                        getBack.Add(mm);
                    }
                }
            }
            if (tri)
            {
                foreach (var item in counter)
                {
                    if (item.Value >=0 && item.Value<3)
                    {
                        GeometryModel3D mm = findModelsById[item.Key];
                        DiffuseMaterial material = (DiffuseMaterial)mm.Material;
                        SolidColorBrush brush = (SolidColorBrush)material.Brush;
                        brush.Color = Color.FromArgb(0, brush.Color.R, brush.Color.G, brush.Color.B);
                        getBack.Add(mm);
                    }
                }
            }
            if (izmedju)
            {
                foreach (var item in counter)
                {
                    if (item.Value >= 3 && item.Value < 5)
                    {
                        GeometryModel3D mm = findModelsById[item.Key];
                        DiffuseMaterial material = (DiffuseMaterial)mm.Material;
                        SolidColorBrush brush = (SolidColorBrush)material.Brush;
                        brush.Color = Color.FromArgb(0, brush.Color.R, brush.Color.G, brush.Color.B);
                        getBack.Add(mm);
                    }
                }
            }
            bool back = comboBox.SelectedItem.ToString().Contains("back");
            if (back)
            {
                foreach (var item in getBack)
                {
                    GeometryModel3D mm = item;
                    DiffuseMaterial material = (DiffuseMaterial)mm.Material;
                    SolidColorBrush brush = (SolidColorBrush)material.Brush;
                    brush.Color = Color.FromRgb(brush.Color.R, brush.Color.G, brush.Color.B);
                }
                getBack.Clear();
            }

            
        }

        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
		{
			bool isNorthHemisphere = true;

			var diflat = -0.00066286966871111111111111111111111111;
			var diflon = -0.0003868060578;

			var zone = zoneUTM;
			var c_sa = 6378137.000000;
			var c_sb = 6356752.314245;
			var e2 = Math.Pow(Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2), 0.5) / c_sb;
			var e2cuadrada = Math.Pow(e2, 2);
			var c = Math.Pow(c_sa, 2) / c_sb;
			var x = utmX - 500000;
			var y = isNorthHemisphere ? utmY : utmY - 10000000;

			var s = (zone * 6.0) - 183.0;
			var lat = y / (c_sa * 0.9996);
			var v = c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5) * 0.9996;
			var a = x / v;
			var a1 = Math.Sin(2 * lat);
			var a2 = a1 * Math.Pow(Math.Cos(lat), 2);
			var j2 = lat + (a1 / 2.0);
			var j4 = ((3 * j2) + a2) / 4.0;
			var j6 = ((5 * j4) + Math.Pow(a2 * Math.Cos(lat), 2)) / 3.0;
			var alfa = 3.0 / 4.0 * e2cuadrada;
            var beta = 5.0 / 3.0 * Math.Pow(alfa, 2);
			var gama = 35.0 / 27.0 * Math.Pow(alfa, 3);
			var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
			var b = (y - bm) / v;
			var epsi = e2cuadrada * Math.Pow(a, 2) / 2.0 * Math.Pow(Math.Cos(lat), 2);
			var eps = a * (1 - (epsi / 3.0));
			var nab = (b * (1 - epsi)) + lat;
			var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
			var delt = Math.Atan(senoheps / Math.Cos(nab));
			var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

			longitude = (delt * (180.0 / Math.PI)) + s + diflon;
			latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - 3.0 / 2.0 * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
		}
		#endregion
	}
}
