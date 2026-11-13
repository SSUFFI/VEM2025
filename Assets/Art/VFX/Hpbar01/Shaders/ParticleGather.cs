
using UnityEngine;

public class ParticleGather : MonoBehaviour
{
    public ParticleSystem ps;
    public Transform targetPoint;

    [Header("Gather Settings")]
    public float gatherSpeed = 15f;

    [Header("Delay Settings")]
    public float gatherDelay = 0.5f;

    private float gatherTimer = 0f;

    private ParticleSystem.Particle[] particles;

    private void LateUpdate()
    {
        if (ps == null || targetPoint == null)
            return;

        int max = ps.main.maxParticles;

        if (particles == null || particles.Length < max)
            particles = new ParticleSystem.Particle[max];

        int count = ps.GetParticles(particles);

        if (count == 0)
        {
            gatherTimer = 0f;
            return;
        }

        gatherTimer += Time.deltaTime;

        if (gatherTimer < gatherDelay)
            return;

        Vector3 target = targetPoint.position;

        for (int i = 0; i < count; i++)
        {
            Vector3 current = particles[i].position;

            // 현재 위치에서 타겟 방향으로 이동
            particles[i].position = Vector3.MoveTowards(
                current,
                target,
                gatherSpeed * Time.deltaTime
            );

            // 기존 속도로 인해 위치가 다시 변하지 않도록 설정
            particles[i].velocity = Vector3.zero;
        }

        ps.SetParticles(particles, count);
    }
    public void ResetGatherDelay()
    {
        gatherTimer = 0f;
    }

    public void RestartEffect()
    {
        if (ps == null)
            return;

        // 기존 파티클 전부 제거
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Clear(true);

        // 빨려 들어가는 대기 시간 초기화
        gatherTimer = 0f;

        // 파티클 시스템 처음부터 다시 실행
        ps.Play(true);
    }
}
