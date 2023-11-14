using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows;

namespace TW2D
{
    public class Game
    {
        public Tank Tank1 { get; set; }
        public Tank Tank2 { get; set; }
        public Tank CurrentTank { get; private set; }
        public Game()
        {
            Tank1 = new Tank(new Point(50, 100), 5, 100, Brushes.Blue, "Tank1");
            Tank2 = new Tank(new Point(850, 100), 5,100, Brushes.Red, "Tank2");
            CurrentTank = Tank1;
        }
        public void SetCurrentTank()
        {
            if(CurrentTank == Tank1)
            {
                CurrentTank = Tank2;
            }
            else
            {
                CurrentTank = Tank1;
            }
        }
        public Tank GetAnotherTank()
        {
            if (CurrentTank == Tank1)
                return Tank2;
            return Tank1;
        }
    }
    public class Tank
    {
        public Point Location { get; set; }
        public int HitPoints { get; set; }
        public EllipseGeometry Body { get; set; }
        public Path Model { get; set; }
        public int Fuel { get; set; }
        public Tank(Point location, int hp,int fuel, Brush brushes, string name)
        {
            Location = location;
            HitPoints = hp;
            Fuel = fuel;
            Body = new EllipseGeometry() { Center = location, RadiusX = 20, RadiusY = 20};
            var path = new Path();
            path.Data = Body;
            path.Fill = brushes;
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 1;
            path.Name = name;
            Model = path;
        }
        
    }
    public class Bullet
    {
        
        public EllipseGeometry HitBox { get; set; }
        public Bullet(Point location)
        {
            HitBox = new EllipseGeometry() { Center = location, RadiusX = 30, RadiusY = 30 };
        }
    }
    public class Trajectory
    {
        public PathFigure Figure = new PathFigure();       
        public BezierSegment Bezier = new BezierSegment();                
        PathGeometry PathGeom = new PathGeometry();       
        public Path Model = new Path();

        public Trajectory(Tank tank)
        {
            var x = tank.Body.Center.X;
            var y = tank.Body.Center.Y - 20;
            Figure.IsClosed = false;
            Figure.StartPoint = new Point(x, y);
            Bezier.Point1 = new Point(x, y);
            Bezier.Point2 = new Point(x, y);
            Bezier.Point3 = new Point(x, 450);
            Figure.Segments.Add(Bezier);
            PathGeom.Figures.Add(Figure);
            Model.Stroke = Brushes.White;
            Model.StrokeThickness = 3;
            Model.Data = PathGeom;
        }
        public void SetDefaultTrajectory(Tank tank)
        {
            var x = tank.Body.Center.X;
            var y = tank.Body.Center.Y - 20;
            Figure.StartPoint = new Point(x, y);
            Bezier.Point1 = new Point(x, y);
            Bezier.Point2 = new Point(x, y);
            Bezier.Point3 = new Point(x, 450);
        }
        public void ChangeTrajectory(Tank tank, int xAdd, int yAdd)
        {
            var x = tank.Body.Center.X;
            var y = tank.Body.Center.Y - 20;
            Figure.StartPoint = new Point(x, y);
            Bezier.Point3 = new Point(Bezier.Point3.X + xAdd, 449);
            Bezier.Point1 = new Point(Math.Abs(Bezier.Point3.X + x) / 2, Bezier.Point1.Y + yAdd);
            Bezier.Point2 = new Point(Math.Abs(Bezier.Point3.X + x) / 2, Bezier.Point2.Y + yAdd);
            
        }
    }
}
