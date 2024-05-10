﻿using System;

namespace Ursa.Music._163.Utils;

public class SecondOrderDynamicsForArray
{
    private double _f;
    private double _r;
    private double _w, _z, _d, k1, k2, k3; // dynamics constants
    private double[] xps, xds; // previous input
    private double[] ys, yds; // state variables

    /// <summary>
    /// 
    /// </summary>
    /// <param name="f"></param>
    /// <param name="z"></param>
    /// <param name="r"></param>
    /// <param name="x0"></param>
    /// <param name="size">Array size</param>
    public SecondOrderDynamicsForArray(double f, double z, double r, double x0, int size)
    {
        //compute constants
        InitMotionValues(f, z, r);

        // initialize variables
        xps = new double[size];
        ys = new double[size];

        xds = new double[size];
        yds = new double[size];

        Array.Fill(xps, x0);
        Array.Fill(ys, x0);
    }

    /// <summary>
    /// 频率
    /// - 即速度, 单位是赫兹(Hz)
    /// - 不会影响输出结果的形状, 会影响 '震荡频率'
    /// </summary>
    public double F
    {
        get => _f;
        set
        {
            _f = value;
            InitMotionValues(_f, _z, _r);
        }
    }

    /// <summary>
    /// 阻尼 <br />
    /// - 当为 0 时, 输出将永远震荡不衰减 <br />
    /// - 当大于 0 小于 1 时, 输出会超出结果, 并逐渐趋于目标 <br />
    /// - 当为 1 时, 输出的曲线是趋向结果, 并正好在指定频率对应时间内抵达结果 <br />
    /// - 当大于 1 时, 输出值同样时取向结果, 但速度会更慢, 无法在指定频率对应时间内抵达结果 <br />
    /// </summary>
    public double Z
    {
        get => _z;
        set
        {
            _z = value;
            InitMotionValues(_f, _z, _r);
        }
    }

    /// <summary>
    /// 初始响应
    /// - 当为 0 时, 数据需要进行 '加速' 来开始运动 <br />
    /// - 当为 1 时, 数据会立即开始响应 <br />
    /// - 当大于 1 时, 输出会因为 '速度过快' 而超出目标结果  <br />
    /// - 当小于 0 时, 输出会 '预测运动', 即 '抬手动作'. 例如目标是 '加' 时, 输出会先进行 '减', 再进行 '加', 
    /// - 当运动目标为机械时, 通常取值为 2
    /// </summary>
    public double R
    {
        get => _r;
        set
        {
            _r = value;
            InitMotionValues(_f, _z, _r);
        }
    }

    private void InitMotionValues(double f, double z, double r)
    {
        _w = 2 * Math.PI * f;
        _z = z;
        _d = _w * Math.Sqrt(Math.Abs(z * z - 1));
        k1 = z / (Math.PI * f);
        k2 = 1 / ((2 * Math.PI * f) * (2 * Math.PI * f));
        k3 = r * z / (2 * Math.PI * f);
    }

    public double[] Update(double deltaTime, double[] xs)
    {
        if (xs.Length != xps.Length)
            throw new ArgumentException();

        for (int i = 0; i < xds.Length; i++)
            xds[i] = (xs[i] - xps[i]) / deltaTime;
        double k1_stable, k2_stable;
        if (_w * deltaTime < _z)
        {
            k1_stable = k1;
            k2_stable = Math.Max(Math.Max(k2, deltaTime * deltaTime / 2 + deltaTime * k1 / 2), deltaTime * k1);
        }
        else
        {
            double t1 = Math.Exp(-_z * _w * deltaTime);
            double alpha = 2 * t1 * (_z <= 1 ? Math.Cos(deltaTime * _d) : Math.Cosh(deltaTime * _d));
            double beta = t1 * t1;
            double t2 = deltaTime / (1 + beta - alpha);
            k1_stable = (1 - beta) * t2;
            k2_stable = deltaTime * t2;
        }

        for (int i = 0; i < ys.Length; i++)
        {
            ys[i] = ys[i] + deltaTime * yds[i];
            yds[i] = yds[i] + deltaTime * (xs[i] + k3 * xds[i] - ys[i] - k1_stable * yds[i]) / k2_stable;
        }

        for (int i = 0; i < xps.Length; i++)
            xps[i] = xs[i];
        return ys;
    }
}