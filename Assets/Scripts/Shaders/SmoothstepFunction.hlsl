//from https://iquilezles.org/articles/smoothsteps/


void PiecewiseQuad_float(in float x,out float outVal)
{
	outVal = (x<0.5) ? 2.0*x*x : 2.0*x*(2.0-x)-1.0;
}

float PiecewiseQuad(float x)
{
	return (x<0.5) ? 2.0*x*x : 2.0*x*(2.0-x)-1.0;
}

void PiecewisePoly_float(in float x,in float n,out float outVal)
{
	outVal = (x<0.5) ? 0.5*pow(2.0*x,n):1.0-0.5*pow(2.0*(1.0-x), n);
}

void CubicPoly_float(in float x,out float outVal)
{
	outVal = x*x*(3.0-2.0*x);
}

float CubicPoly(float x)
{
	return x*x*(3.0-2.0*x);
}

void QuarticPoly_float(in float x,out float outVal)
{
	outVal = x*x*(2.0-x*x);
}

float QuarticPoly(float x)
{
	return x*x*(2.0-x*x);
}

void QuinticPoly_float(in float x,out float outVal)
{
	outVal = x*x*x*(x*(x*6.0-15.0)+10.0);
}

float QuinticPoly(float x)
{
	return x*x*x*(x*(x*6.0-15.0)+10.0);
}

void QuadRat_float(in float x,out float outVal)
{
	outVal = x*x/(2.0*x*x-2.0*x+1.0);
}

float QuadRat(float x)
{
	return x*x/(2.0*x*x-2.0*x+1.0);
}

void CubicRat_float(in float x,out float outVal)
{
	outVal = x*x*x/(3.0*x*x-3.0*x+1.0);
}

float CubicRat(float x)
{
	return x*x*x/(3.0*x*x-3.0*x+1.0);
}

void Rat_float(in float x,in float n,out float outVal)
{
	outVal = pow(x,n)/(pow(x,n)+pow(1.0-x,n));
}

void Trig_float(in float x,out float outVal)
{
	outVal = 0.5-0.5*cos(PI*x);
}

float Trig(float x)
{
	return 0.5-0.5*cos(PI*x);
}

void SmoothStep_float(in float x,in int i,out float outVal)
{
	switch(i)
	{
		case 0:
			outVal = x;
			return;
		case 1:
			outVal = PiecewiseQuad(x);
			return;
		case 2:
			outVal = CubicPoly(x);
			return;
		case 3:
			outVal = QuarticPoly(x);
			return;
		case 4:
			outVal =  QuinticPoly(x);
			return;
		case 5:
			outVal = QuadRat(x);
			return;
		case 6:
			outVal = CubicRat(x);
			return;
		case 7:
			outVal = Trig(x);
			return;
		default:
			outVal = x;
			return;
	}
}
