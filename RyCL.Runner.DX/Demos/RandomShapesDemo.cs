using RyCL.Runner.DX.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyCL.Runner.DX.Demos
{
    //public class RandomShapesDemo : ClientCL
    //{
    //    private Action _shape;
    //    public RandomShapesDemo() :
    //        base()
    //    { }

    //    protected override void Initialize(ClientConfiguration configuration)
    //    {
    //        base.Initialize(configuration);

    //        _shape = DrawRandom;
    //    }
    //    protected override void Update(Time time)
    //    {
    //        base.Update(time);
    //        CheckInput();
    //        _shape();
    //    }

    //    private void CheckInput()
    //    {
    //        if (_keys[Keys.D1])      { _graphics.Fill(0); _shape = DrawRandom; }
    //        else if (_keys[Keys.D2]) { _graphics.Fill(0); _shape = DrawLines; }
    //        else if (_keys[Keys.D3]) { _graphics.Fill(0); _shape = DrawTriangles; }
    //        else if (_keys[Keys.D4]) { _graphics.Fill(0); _shape = DrawRectangles; }
    //        else if (_keys[Keys.D5]) { _graphics.Fill(0); _shape = DrawCircles; }
    //        else if (_keys[Keys.D6]) { _graphics.Fill(0); _shape = FillTriangles; }
    //        else if (_keys[Keys.D7]) { _graphics.Fill(0); _shape = FillRectangles; }
    //        else if (_keys[Keys.D8]) { _graphics.Fill(0); _shape = FillCircles; }
    //        else if (_keys[Keys.D9]) { _graphics.Fill(0); _shape = FillRandom; }
    //    }

    //    private void DrawRandom()
    //    {
    //        _graphics.FillRandom();
    //    }
    //    private void DrawLines()
    //    {
    //        var x0 = _random.Next(0, Width);
    //        var y0 = _random.Next(0, Height);
    //        var x1 = _random.Next(0, Width);
    //        var y1 = _random.Next(0, Height);
    //        var color = _random.Next() | (0xFF << 24);
    //        _graphics.DrawLine(x0, y0, x1, y1, color);
    //    }
    //    private void DrawTriangles()
    //    {
    //        var x0 = _random.Next(0, Width);
    //        var y0 = _random.Next(0, Height);
    //        var x1 = _random.Next(0, Width);
    //        var y1 = _random.Next(0, Height);
    //        var x2 = _random.Next(0, Width);
    //        var y2 = _random.Next(0, Height);
    //        var color = _random.Next() | (0xFF << 24);
    //        _graphics.DrawTriangle(x0, y0, x1, y1, x2, y2, color);
    //    }
    //    private void DrawRectangles()
    //    {
    //        var x0 = _random.Next(0, Width);
    //        var y0 = _random.Next(0, Height);
    //        var x1 = _random.Next(0, Width);
    //        var y1 = _random.Next(0, Height);
    //        var color = _random.Next() | (0xFF << 24);
    //        _graphics.DrawRectangle(x0, y0, x1, y1, color);
    //    }
    //    private void DrawCircles()
    //    {
    //        var x = _random.Next(0, Width);
    //        var y = _random.Next(0, Height);
    //        var r = _random.Next(0, Width >> 2);
    //        var color = _random.Next() | (0xFF << 24);
    //        _graphics.DrawCircle(x, y, r, color);
    //    }
    //    private void FillTriangles()
    //    {
    //        var x0 = _random.Next(0, Width);
    //        var y0 = _random.Next(0, Height);
    //        var x1 = _random.Next(0, Width);
    //        var y1 = _random.Next(0, Height);
    //        var x2 = _random.Next(0, Width);
    //        var y2 = _random.Next(0, Height);
    //        var color = _random.Next() | (0xFF << 24);
    //        _graphics.FillTriangle(x0, y0, x1, y1, x2, y2, color);
    //    }
    //    private void FillRectangles()
    //    {
    //        var x0 = _random.Next(0, Width);
    //        var y0 = _random.Next(0, Height);
    //        var x1 = _random.Next(0, Width);
    //        var y1 = _random.Next(0, Height);
    //        var color = _random.Next() | (0xFF << 24);
    //        _graphics.FillRectangle(x0, y0, x1, y1, color);
    //    }
    //    private void FillCircles()
    //    {
    //        var x = _random.Next(0, Width);
    //        var y = _random.Next(0, Height);
    //        var r = _random.Next(0, Width >> 2);
    //        var color = _random.Next() | (0xFF << 24);
    //        _graphics.FillCircle(x, y, r, color);
    //    }
    //    private void FillRandom()
    //    {
    //        var color = _random.Next() | (0xFF << 24);
    //        _graphics.Fill(color);
    //    }
    //}
}
