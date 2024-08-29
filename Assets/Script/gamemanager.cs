using UnityEngine;
using System.Collections;
using static Unity.Collections.AllocatorManager;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform[] transformcell; // Mảng chứa các vị trí trên bảng
    public GameObject blockPrefab; // Prefab của khối
    public float moveDuration = 4f;// Thời gian di chuyển
    private bool isMoving = false;
    private bool cotaokhoi = false;
    void Start()
    {
        blurPanel.SetActive(false);
        loseText.enabled = false;
        winText.enabled = false;
        // Gọi hàm tạo khối trong Start hoặc nơi nào bạn muốn
        taokhoi();
        taokhoi();
    }

    void Update()
    {
        if (isGameOver) return;
        if (!isMoving && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            StartCoroutine(HandleMove(MoveUp));
        }
        else if (!isMoving && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            StartCoroutine(HandleMove(MoveLeft));
        }
        else if (!isMoving && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            StartCoroutine(HandleMove(MoveDown));
        }
        else if (!isMoving && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            StartCoroutine(HandleMove(MoveRight));
        }
    }

    void taokhoi()
    {
        int randomNumber = Random.Range(1, 3); // Random.Range(1, 3) sẽ sinh ra số ngẫu nhiên 1 hoặc 2

        // Tìm một cell ngẫu nhiên mà chưa có khối
        Transform cellTransform = null;
        while (cellTransform == null)
        {
            int randomNumberkhoi = Random.Range(0, transformcell.Length); // Random.Range(0, transformcell.Length) sẽ sinh ra số ngẫu nhiên từ 0 đến độ dài của mảng transformcell
            if (transformcell[randomNumberkhoi].childCount == 0)
            {
                cellTransform = transformcell[randomNumberkhoi];
                cellscript currentCelll = cellTransform.GetComponent<cellscript>();
                currentCelll.coblock = true;
            }
        }

        // Tạo khối mới tại vị trí của cell và đặt nó làm con của cell
        GameObject newBlock = Instantiate(blockPrefab, cellTransform.position, Quaternion.identity, cellTransform);

        // Gán giá trị cho khối mới
        cubescript cubeScript = newBlock.GetComponent<cubescript>();

        Canvas canvas = newBlock.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = newBlock.AddComponent<Canvas>();
        }

        canvas.overrideSorting = true;
        canvas.sortingOrder = 1;

        if (randomNumber == 1)
        {
            cubeScript.SetValue(2);
            Debug.Log("Tạo khối 2 tại vị trí " + System.Array.IndexOf(transformcell, cellTransform));
        }
        else
        {
            cubeScript.SetValue(4);
            Debug.Log("Tạo khối 4 tại vị trí " + System.Array.IndexOf(transformcell, cellTransform));
        }
    }


    void MoveLeft()
    {
        // Di chuyển các khối từ trái sang phải
        for (int i = 0; i < transformcell.Length; i++)
        {
            cellscript currentCell = transformcell[i].GetComponent<cellscript>();
            if (currentCell != null)
            {
                cubescript currentBlock = transformcell[i].GetComponentInChildren<cubescript>();

                if (currentBlock != null)
                {
                    cellscript nextCell = currentCell.left;
                    while (nextCell != null)
                    {
                        cubescript nextBlock = nextCell.GetComponentInChildren<cubescript>();

                        if (!nextCell.coblock)
                        {
                            // Đặt lại trạng thái của cell hiện tại
                            currentCell.coblock = false;

                            // Đặt trạng thái cho cell tiếp theo
                            nextCell.coblock = true;

                            // Di chuyển khối sang trái
                            StartCoroutine(MoveBlock(currentBlock.transform, nextCell.transform.position));
                            currentBlock.transform.SetParent(nextCell.transform);

                            currentCell = nextCell;
                            // Tiếp tục kiểm tra cell tiếp theo
                            nextCell = nextCell.left;
                        }
                        else if (nextCell.coblock)
                        {
                            if (nextBlock.value == currentBlock.value && nextBlock.dahopnhat == 0)
                            {
                                // Hợp nhất khối
                                currentCell.coblock = false;
                                nextBlock.dahopnhat = 1;

                                StartCoroutine(MoveBlockk(currentBlock.transform, nextCell.transform.position, nextBlock));

                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Đặt lại trạng thái `dahopnhat` cho các khối sau khi di chuyển
            if (i == transformcell.Length - 1)
            {
                for (int j = 0; j < transformcell.Length; j++)
                {
                    cubescript currentBlock = transformcell[j].GetComponentInChildren<cubescript>();
                    if (currentBlock != null)
                    {
                        currentBlock.dahopnhat = 0;
                    }
                    else
                    {
                        // Đảm bảo cell không có block được đặt `coblock = false`
                    }
                }
            }
        }
    }


    IEnumerator HandleMove(System.Action moveAction)
    {
        isMoving = true; // Set the flag to true to indicate movement is in progress

        // Gọi hành động di chuyển
        moveAction.Invoke();

        // Wait for the movement and merging animations to complete
        yield return new WaitForSeconds(moveDuration*1.3f);

        isMoving = false; // Reset the flag after the move is complete
        CheckWin();
        CheckGameOver();
        
        if(cotaokhoi)
        {
            taokhoi();
        }
        else
        {

        }
        cotaokhoi = false;
       // taokhoi();
    }

    void MoveUp()
    {
        
        // Di chuyển các khối từ trên xuống dưới
        for (int i = 0; i < transformcell.Length; i++)
        {
            cellscript currentCell = transformcell[i].GetComponent<cellscript>();
            if (currentCell != null)
            {
                cubescript currentBlock = transformcell[i].GetComponentInChildren<cubescript>();

                if (currentBlock != null)
                {
                    cellscript nextCell = currentCell.up;
                    while (nextCell != null)
                    {
                        cubescript nextBlock = nextCell.GetComponentInChildren<cubescript>();

                        if (!nextCell.coblock)
                        {
                            // Đặt lại trạng thái của cell hiện tại
                            currentCell.coblock = false;

                            // Đặt trạng thái cho cell tiếp theo
                            nextCell.coblock = true;

                            // Di chuyển khối lên
                            StartCoroutine(MoveBlock(currentBlock.transform, nextCell.transform.position));
                            currentBlock.transform.SetParent(nextCell.transform);

                            currentCell = nextCell;
                            // Tiếp tục kiểm tra cell tiếp theo
                            nextCell = nextCell.up;
                        }
                        else if (nextCell.coblock)
                        {
                            if (nextBlock.value == currentBlock.value && nextBlock.dahopnhat == 0)
                            {
                                // Hợp nhất khối
                                currentCell.coblock = false;
                                nextBlock.dahopnhat = 1;

                                StartCoroutine(MoveBlockk(currentBlock.transform, nextCell.transform.position, nextBlock));

                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Đặt lại trạng thái `dahopnhat` cho các khối sau khi di chuyển
            if (i == transformcell.Length - 1)
            {
                for (int j = 0; j < transformcell.Length; j++)
                {
                    cubescript currentBlock = transformcell[j].GetComponentInChildren<cubescript>();
                    if (currentBlock != null)
                    {
                        currentBlock.dahopnhat = 0;
                    }
                    else
                    {
                        // Đảm bảo cell không có block được đặt `coblock = false`
                    }
                }
            }
        }
    }

    void MoveDown()
    {
        // Di chuyển các khối từ dưới lên trên
        for (int i = transformcell.Length - 1; i >= 0; i--)
        {
            cellscript currentCell = transformcell[i].GetComponent<cellscript>();
            if (currentCell != null)
            {
                cubescript currentBlock = transformcell[i].GetComponentInChildren<cubescript>();

                if (currentBlock != null)
                {
                    cellscript nextCell = currentCell.down;
                    while (nextCell != null)
                    {
                        cubescript nextBlock = nextCell.GetComponentInChildren<cubescript>();

                        if (!nextCell.coblock)
                        {
                            // Đặt lại trạng thái của cell hiện tại
                           currentCell.coblock = false;

                            // Đặt trạng thái cho cell tiếp theo
                            nextCell.coblock = true;

                            // Di chuyển khối xuống
                            StartCoroutine(MoveBlock(currentBlock.transform, nextCell.transform.position));
                            currentBlock.transform.SetParent(nextCell.transform);

                           

                            currentCell = nextCell;
                            // Tiếp tục kiểm tra cell tiếp theo
                            nextCell = nextCell.down;



                           
                        }
                        else if (nextCell.coblock)
                        {
                            if (nextBlock.value == currentBlock.value && nextBlock.dahopnhat == 0)
                            {
                                // Hợp nhất khối
                               currentCell.coblock = false;
                              nextBlock.dahopnhat = 1;

                                //   StartCoroutine(MoveBlock(currentBlock.transform, nextCell.transform.position));
                                // StartCoroutine(MergeBlocks(currentBlock, nextBlock));

                                StartCoroutine(MoveBlockk(currentBlock.transform, nextCell.transform.position, nextBlock));

                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Đặt lại trạng thái `dahopnhat` cho các khối sau khi di chuyển
            if (i == 0)
            {
                for (int j = transformcell.Length - 1; j >= 0; j--)
                {
                    cubescript currentBlock = transformcell[j].GetComponentInChildren<cubescript>();
                    if (currentBlock != null)
                    {
                        currentBlock.dahopnhat = 0;
                    }
                    else
                    {
                        // Đảm bảo cell không có block được đặt `coblock = false`
                       
                    }
                }
            }
        }
    }


    IEnumerator MoveBlock(Transform block, Vector3 targetPosition)
    {
        if (block == null)
            yield break;

        Vector3 startPosition = block.position;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            if (block == null)
                yield break;

            block.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (block != null)
        {
            block.position = targetPosition;

            
        }
        cotaokhoi = true;
    }

    IEnumerator MoveBlockk(Transform block, Vector3 targetPosition, cubescript block2)
    {
        if (block == null)
            yield break;

        Vector3 startPosition = block.position;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            if (block == null)
                yield break;

            block.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (block != null)
        {
            block.position = targetPosition;
        }


        if(block == null || block2 == null)
            
            yield break;

        // Di chuyển block1 tới vị trí của block2
        

        if (block == null || block2 == null)
            yield break;

        // Tăng giá trị của block2 và cập nhật giao diện
        block2.SetValue(block2.value * 2);

        // Giới thiệu một thời gian trì hoãn nhỏ trước khi hủy
        yield return new WaitForSeconds(0.1f);

        if (block != null)
        {
            // Xóa block1 khỏi trò chơi
            Destroy(block.gameObject);
        }
        cotaokhoi = true;
    }





    void MoveRight()
    {
        // Di chuyển các khối từ phải sang trái
        for (int i = transformcell.Length - 1; i >= 0; i--)
        {
            cellscript currentCell = transformcell[i].GetComponent<cellscript>();
            if (currentCell != null)
            {
                cubescript currentBlock = transformcell[i].GetComponentInChildren<cubescript>();

                if (currentBlock != null)
                {
                    cellscript nextCell = currentCell.right;
                    while (nextCell != null)
                    {
                        cubescript nextBlock = nextCell.GetComponentInChildren<cubescript>();

                        if (!nextCell.coblock)
                        {
                            // Đặt lại trạng thái của cell hiện tại
                            currentCell.coblock = false;

                            // Đặt trạng thái cho cell tiếp theo
                            nextCell.coblock = true;

                            // Di chuyển khối sang phải
                            StartCoroutine(MoveBlock(currentBlock.transform, nextCell.transform.position));
                            currentBlock.transform.SetParent(nextCell.transform);

                            currentCell = nextCell;
                            // Tiếp tục kiểm tra cell tiếp theo
                            nextCell = nextCell.right;
                        }
                        else if (nextCell.coblock)
                        {
                            if (nextBlock.value == currentBlock.value && nextBlock.dahopnhat == 0)
                            {
                                // Hợp nhất khối
                                currentCell.coblock = false;
                                nextBlock.dahopnhat = 1;

                                StartCoroutine(MoveBlockk(currentBlock.transform, nextCell.transform.position, nextBlock));

                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Đặt lại trạng thái `dahopnhat` cho các khối sau khi di chuyển
            if (i == 0)
            {
                for (int j = transformcell.Length - 1; j >= 0; j--)
                {
                    cubescript currentBlock = transformcell[j].GetComponentInChildren<cubescript>();
                    if (currentBlock != null)
                    {
                        currentBlock.dahopnhat = 0;
                    }
                    else
                    {
                        // Đảm bảo cell không có block được đặt `coblock = false`
                    }
                }
            }
        }
    }

    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;
    private bool isGameOver = false;
    public GameObject blurPanel;

    void CheckGameOver()
    {
        if (isGameOver) return; // Ngăn chặn tiếp tục kiểm tra nếu game đã kết thúc

        bool hasEmptyCell = false;

        // Kiểm tra xem có ô trống nào không
        foreach (Transform cellTransform in transformcell)
        {
            cellscript cell = cellTransform.GetComponent<cellscript>();
            if (!cell.coblock)
            {
                hasEmptyCell = true;
                break;
            }
        }

        if (!hasEmptyCell)
        {
            // Kiểm tra xem có thể hợp nhất khối nào không (nếu không còn ô trống)
            foreach (Transform cellTransform in transformcell)
            {
                cubescript currentBlock = cellTransform.GetComponentInChildren<cubescript>();
                if (currentBlock != null)
                {
                    cellscript currentCell = cellTransform.GetComponent<cellscript>();

                    // Kiểm tra khả năng hợp nhất với các ô lân cận (trên, dưới, trái, phải)
                    if (currentCell.left != null && currentCell.left.GetComponentInChildren<cubescript>() != null && currentCell.left.GetComponentInChildren<cubescript>().value == currentBlock.value ||
                        currentCell.right != null && currentCell.right.GetComponentInChildren<cubescript>() != null && currentCell.right.GetComponentInChildren<cubescript>().value == currentBlock.value ||
                        currentCell.up != null && currentCell.up.GetComponentInChildren<cubescript>() != null && currentCell.up.GetComponentInChildren<cubescript>().value == currentBlock.value ||
                        currentCell.down != null && currentCell.down.GetComponentInChildren<cubescript>() != null && currentCell.down.GetComponentInChildren<cubescript>().value == currentBlock.value)
                    {
                        return; // Còn khả năng hợp nhất, không thua
                    }
                }
            }
            blurPanel.SetActive(true);
            // Nếu không còn ô trống và không thể hợp nhất, bạn thua
            loseText.enabled = true; // Hiển thị chữ "Lose"
            Debug.Log("Bạn đã thua!");
            isGameOver = true; // Khóa màn hình không nhận nút nữa

            Invoke("ReplayGame", 2f);
        }
    }

    void CheckWin()
    {
        if (isGameOver) return; // Ngăn chặn tiếp tục kiểm tra nếu game đã kết thúc

        foreach (Transform cellTransform in transformcell)
        {
            cubescript currentBlock = cellTransform.GetComponentInChildren<cubescript>();
            if (currentBlock != null && currentBlock.value == 2048)
            {
                blurPanel.SetActive(true);
                winText.enabled = true; // Hiển thị chữ "Win"
                Debug.Log("Bạn đã thắng!");
                isGameOver = true; // Khóa màn hình không nhận nút nữa

                Invoke("ReplayGame", 2f);
                return;
            }
        }
    }

    public GameObject replayButton;
    public void ReplayGame()
    {
        isGameOver = false;
        blurPanel.SetActive(false);
        winText.enabled = false;
        loseText.enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
