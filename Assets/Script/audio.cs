using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource; // Đối tượng AudioSource
    public Slider volumeSlider; // Slider để điều chỉnh âm lượng
    public Toggle muteToggle; // Toggle để bật/tắt nhạc

    void Start()
    {
        // Thiết lập âm lượng ban đầu và trạng thái tắt tiếng
        audioSource.volume = volumeSlider.value;
        audioSource.mute = muteToggle.isOn;

        // Bắt đầu phát nhạc nền
        audioSource.Play();

        // Gắn các sự kiện khi điều chỉnh slider và toggle
        volumeSlider.onValueChanged.AddListener(SetVolume);
        muteToggle.onValueChanged.AddListener(ToggleMute);
    }

    // Hàm để điều chỉnh âm lượng
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    // Hàm để bật/tắt nhạc nền
    public void ToggleMute(bool isMuted)
    {
        audioSource.mute = isMuted;
    }
}
