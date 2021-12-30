using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BrickObject : MonoBehaviour
{
    [SerializeField] Slider slider;
    public UnityEvent<int> OnDestroyed;
    public UnityEvent<int> OnBrickHitted;
    int lives;
    BrickData data;
    int id;
    public void Initialize(BrickData data, int lives, int id)
    {
        this.data = data;
        this.lives = lives;
        this.id = id;
        Colorize();
        InitializeSlider();
    }
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    private void Colorize()
    {

    }
    private void InitializeSlider()
    {
        if (data.IsIndestructible)
        {
            Destroy(slider.gameObject);
            return;
        }
        slider.minValue = 0;
        slider.maxValue = data.HitsToDestroy;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        slider.value = lives;
    }

    private void Hit()
    {
        if (data.IsIndestructible)
            return;
        lives--;
        UpdateSlider();
        if (lives <= 0)
            Break();
        else
            OnBrickHitted?.Invoke(id);
    }
    private void Break()
    {
        OnDestroyed?.Invoke(data.Points);
        Destroy(gameObject, 0.05f);
    }
    private void OnCollisionEnter(Collision other)
    {
        Hit();
    }
}