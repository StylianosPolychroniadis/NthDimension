// CloudSphere.h
//


class CCloudSphere : public CSRTTransform
{
protected:
	float m_fBoundingRadius;
	int m_nStages;
	CMatrix4 m_mTex1;
	CMatrix4 m_mTex2;
	CMatrix4 m_mTex3;
	CMatrix4 m_mTex4;

public:
	CCloudSphere()
	{
		m_nStages = 4;
		m_mTex1.IMatrix();
		m_mTex2.IMatrix();
		m_mTex3.IMatrix();
		m_mTex4.IMatrix();
	}
	void SetBoundingRadius(float f)		{ m_fBoundingRadius = f; }
	void SetStages(int n)				{ m_nStages = n; }
	void Draw(const CSRTTransform &camera, CTexture &tex);
};
