using UnityEngine;
using System.Collections; // 코루틴을 사용하기 위해 필요합니다.

// 이 스크립트는 에셋 스토어에서 가져온 폭발 이펙트 프리팹에 붙습니다.
// 이펙트 재생을 관리하고, 재생이 끝나면 스스로 사라지게 합니다.
public class ExplosionEffect : MonoBehaviour
{
    // 이펙트의 총 재생 시간 (이 시간 후 이펙트 GameObject가 파괴됩니다)
    // 에셋 스토어 파티클의 'Duration' 및 'Start Lifetime'을 고려하여 설정하세요.
    public float effectDuration = 1.0f; // Inspector에서 조절 가능

    // 이펙트 프리팹에 붙어있는 ParticleSystem 컴포넌트 (Inspector에서 직접 할당)
    public ParticleSystem explosionParticles;

    // 폭발 사운드 클립 (Inspector에서 직접 할당)
    public AudioClip explosionSound;

    private AudioSource audioSource;

    void Awake()
    {
        // AudioSource 컴포넌트가 없다면 추가하고 가져옵니다.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("[ExplosionEffect] AudioSource 컴포넌트가 없습니다. 자동으로 추가되었습니다. GameObject: " + gameObject.name);
        }
        // ParticleSystem을 Awake에서 자동으로 가져오도록 합니다.
        if (explosionParticles == null)
        {
            explosionParticles = GetComponent<ParticleSystem>();
            if (explosionParticles == null)
            {
                Debug.LogError("[ExplosionEffect] ParticleSystem 컴포넌트가 없습니다. 폭발 이펙트가 재생되지 않습니다! GameObject: " + gameObject.name);
            }
        }
        // [수정] AudioSource의 기본 클립을 설정합니다.
        if (audioSource != null && explosionSound != null)
        {
            audioSource.clip = explosionSound; // 기본 오디오 클립 할당
        }
    }

    // 이펙트 재생 함수: 이펙트 프리팹이 생성될 때 호출됩니다.
    // 위치는 Instantiate 시 이미 설정되므로 여기서는 필요 없습니다.
    public void PlayEffect()
    {
        Debug.Log($"[ExplosionEffect] PlayEffect 호출됨! GameObject: {gameObject.name}"); // 진단 로그 추가

        // 1. 파티클 시스템 재생
        if (explosionParticles != null)
        {
            explosionParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // 기존 파티클 모두 정지 및 제거
            explosionParticles.Play(); // 파티클 재생 시작
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": Explosion Particles (ParticleSystem)이 할당되지 않았습니다. 시각적 이펙트가 보이지 않을 수 있습니다.");
        }

        // 2. 폭발 사운드 재생
        if (audioSource != null && explosionSound != null) // explosionSound가 null인지 다시 확인
        {
            audioSource.PlayOneShot(explosionSound); // 할당된 사운드를 한 번 재생합니다.
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": Explosion Sound AudioClip이 할당되지 않았거나 AudioSource가 없습니다. 사운드가 들리지 않을 수 있습니다.");
        }

        // 3. 지정된 시간 후 이펙트 GameObject를 파괴하는 코루틴 시작
        // 이 코루틴은 Time.timeScale의 영향을 받지 않도록 Unscaled Time을 사용합니다.
        StartCoroutine(DestroyAfterDelay(effectDuration));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        float timer = 0f;
        while (timer < delay)
        {
            // Time.timeScale의 영향을 받지 않도록 Time.unscaledDeltaTime 사용
            timer += Time.unscaledDeltaTime;
            yield return null; // 다음 프레임까지 대기 (Unscaled Time)
        }

        // 이펙트 재생이 끝나면 오브젝트를 파괴합니다.
        Destroy(gameObject);
    }
}
