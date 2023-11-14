using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TW2D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Game Game;
        Path BattleField;
        Trajectory TrajectoryBullet;
        public MainWindow()
        {           
            
            InitializeComponent();
            StartGame();
        }
        public void StartGame()
        {
            Game = new Game();
            TrajectoryBullet = new Trajectory(Game.CurrentTank);
            BattleField = GetBattleField();
            Field.Children.Add(BattleField);
            TankHP1TB.Text = Game.Tank1.HitPoints.ToString();
            TankHP2TB.Text = Game.Tank2.HitPoints.ToString();
        }
        public void IsGameOver()
        {
            if (Game.Tank1.HitPoints == 0)
            {
                GameOver("Проиграл синий");
            }
            if (Game.Tank2.HitPoints == 0)
            {
                GameOver("Проиграл красный");
            }
        }
        public void GameOver(string text)
        {
            GameOverTB.Text = text;
            GameOverTB.Visibility = Visibility.Visible;
            Field.Children.Clear();
            StartGame();
        }
        public void Shoot()
        {            
            PathGeometry pathGeometry = new PathGeometry(new List<PathFigure> {TrajectoryBullet.Figure});
            var list = GetBezierPoints(pathGeometry, (int)(Math.Abs(TrajectoryBullet.Figure.StartPoint.X - TrajectoryBullet.Bezier.Point3.X)));
            Game.SetCurrentTank();
            foreach (var point in list)
            {
                var tankHit = VisualTreeHelper.HitTest(Game.CurrentTank.Model, point);
                if (tankHit != null)
                {                                       
                    TrajectoryBullet.SetDefaultTrajectory(Game.CurrentTank);
                    GetHit(point);
                    Game.CurrentTank.HitPoints--;
                    MoveTank(Game.Tank1, 0);
                    MoveTank(Game.Tank2, 0);
                    break;
                }
                var grountHit = VisualTreeHelper.HitTest(BattleField, point);
                if(grountHit != null)
                {
                    var bullet = new Bullet(point);
                    CombinedGeometry combined = new CombinedGeometry();
                    combined.GeometryCombineMode = GeometryCombineMode.Intersect;
                    combined.Geometry1 = Game.CurrentTank.Body;
                    combined.Geometry2 = bullet.HitBox;
                    if(combined.Bounds != Rect.Empty)
                    {
                        Game.CurrentTank.HitPoints--;
                    }
                    TrajectoryBullet.SetDefaultTrajectory(Game.CurrentTank);
                    GetHit(point);
                    break;
                }               
            }
            TankHP1TB.Text = Game.Tank1.HitPoints.ToString();
            TankHP2TB.Text = Game.Tank2.HitPoints.ToString();
            IsGameOver();
        }
        
        static List<Point> GetBezierPoints(PathGeometry pathGeometry, int numberOfPoints)
        {
            var points = new List<Point>();
            double tIncrement = 1.0 / numberOfPoints;           
            for (double t = 0; t <= 1; t += tIncrement)
            {
                Point point;
                pathGeometry.GetPointAtFractionLength(t, out point, out Point tangent);
                points.Add(point);
            }

            return points;
        }        
        public void MoveTank(Tank tank, int speed)
        {
            var point = tank.Body.Center;
            if (point.Y == 450)
            {
                tank.HitPoints = 0;
                IsGameOver();
                return;
            }              
            var result = VisualTreeHelper.HitTest(BattleField, new Point(point.X, point.Y + 1));
            if (result == null)
            {
                point = FindBattleFieldZ(point.X);
            }
            result = VisualTreeHelper.HitTest(BattleField, point);
            if(result != null)
            {
                for (var z = 450; result != null && z != 0; z--)
                {
                    point.Y--;
                    result = VisualTreeHelper.HitTest(BattleField, point);
                }
            }
            tank.Body.Center = new Point(point.X + speed, point.Y);
        }
        public void GetHit(Point point)
        {
            var bullet = new Bullet(point);
            CombinedGeometry combined = new CombinedGeometry();
            combined.GeometryCombineMode = GeometryCombineMode.Exclude;
            combined.Geometry1 = BattleField.Data;
            combined.Geometry2 = bullet.HitBox;
            BattleField.Data = combined;           
        }
        public Path GetBattleField()
        {
            Field.Background = new SolidColorBrush(Colors.LightSkyBlue);
            PathFigure figure = new PathFigure();
            figure.IsClosed = false;
            figure.StartPoint = new Point(0, 350);
            BezierSegment bezier = new BezierSegment();
            bezier.Point1 = new Point(100, 430);
            bezier.Point2 = new Point(400, 300);
            bezier.Point3 = new Point(800, 350);
            var sdsds = Field.Width;
            PointCollection myPointCollection = new PointCollection(9);
            Random rnd = new Random();
            for(var x = 100; x <= 900; x += 100)
            {
                int y = rnd.Next(300, 400);
                myPointCollection.Add(new Point(x, y));
            }
            PolyBezierSegment myBezierSegment = new PolyBezierSegment();
            myBezierSegment.Points = myPointCollection;
            LineSegment line1 = new LineSegment();
            line1.Point = new Point(900, 450);
            LineSegment line2 = new LineSegment();
            line2.Point = new Point(0, 450);
            LineSegment line3 = new LineSegment();
            line3.Point = new Point(0, 350);
            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection
            {
                myBezierSegment,
                line1,
                line2,
                line3
            };
            figure.Segments = myPathSegmentCollection;
            PathGeometry pathGeom = new PathGeometry();
            pathGeom.Figures.Add(figure);
            Path path = new Path();
            path.Data = pathGeom;
            path.Fill = new SolidColorBrush(Colors.Brown);
            path.Stroke = Brushes.Black;
            path.StrokeThickness =0;
            path.Name = "battleField";
            return path;
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Field.Children.Contains(Game.Tank1.Model))
                {
                    return;
                }
                GameOverTB.Visibility = Visibility.Hidden;
                Field.Children.Add(Game.Tank1.Model);
                Field.Children.Add(Game.Tank2.Model);
                MoveTank(Game.Tank1, 0);
                MoveTank(Game.Tank2, 0);
                TrajectoryBullet.SetDefaultTrajectory(Game.CurrentTank);
                Field.Children.Add(TrajectoryBullet.Model);
            }
            if (e.Key == Key.D)
            {
                TrajectoryBullet.ChangeTrajectory(Game.CurrentTank,2,0);
                MoveTank(Game.CurrentTank, 2);

            }
            if (e.Key == Key.A)
            {
                TrajectoryBullet.ChangeTrajectory(Game.CurrentTank, -2,0);
                MoveTank(Game.CurrentTank, -2);
            }
            if (e.Key == Key.E)
            {
                TrajectoryBullet.ChangeTrajectory(Game.CurrentTank, 3, 0);
            }
            if (e.Key == Key.Q)
            {
                TrajectoryBullet.ChangeTrajectory(Game.CurrentTank, -3, 0);
            }
            if (e.Key == Key.W)
            {
                TrajectoryBullet.ChangeTrajectory(Game.CurrentTank, 0, -3);
            }
            if (e.Key == Key.S)
            {
                TrajectoryBullet.ChangeTrajectory(Game.CurrentTank, 0, 3);
            }
            if (e.Key == Key.F)
            {
                Shoot();
            }
        }
        public Point FindBattleFieldZ(double x)
        {
            var point = new Point(x, 0);
            HitTestResult result = null;
            for(var z = 0; result == null && z != 450; z++)
            {
                point.Y++;
                result = VisualTreeHelper.HitTest(BattleField, point);
            }            
            return point;
        }

    }
}
