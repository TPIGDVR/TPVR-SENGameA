//from https://iquilezles.org/articles/smoothsteps/


void PiecewiseQuad_float(in float x,out float outVal)
{
	outVal = (x<0.5) ? 2.0*x*x : 2.0*x*(2.0-x)-1.0;
}

void PiecewisePoly_float(in float x,in float n,out float outVal)
{
	outVal = (x<0.5) ? 0.5*pow(2.0*x,n):1.0-0.5*pow(2.0*(1.0-x), n);
}

void CubicPoly_float(in float x,out float outVal)
{
	outVal = x*x*(3.0-2.0*x);
}

void QuarticPoly_float(in float x,out float outVal)
{
	outVal = x*x*(2.0-x*x);
}

void QuinticPoly_float(in float x,out float outVal)
{
	outVal = x*x*x*(x*(x*6.0-15.0)+10.0);
}

void QuadRat_float(in float x,out float outVal)
{
	outVal = x*x/(2.0*x*x-2.0*x+1.0);
}

void CubicRat_float(in float x,out float outVal)
{
	outVal = x*x*x/(3.0*x*x-3.0*x+1.0);
}

void Rat_float(in float x,in float n,out float outVal)
{
	outVal = pow(x,n)/(pow(x,n)+pow(1.0-x,n));
}

void Trig_float(in float x,out float outVal)
{
	outVal = 0.5-0.5*cos(PI*x);
}
