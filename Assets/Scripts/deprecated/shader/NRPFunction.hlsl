

void PulseFunction_float(in float t,in float sp,in float sz,in float2 uv,in float i,out float outVal)
{
	int ite = (int)i;
	float totalOut = 0;
	for(int z = 1; z <= ite; z++)
	{
		float x = frac(t * sp * (1 * z));
		float sv = smoothstep(x - sz,x + sz,length(uv - float2(0.5,0.5)));
		totalOut += (1 - sv) * sv;
	}

	outVal = saturate(totalOut);
}

