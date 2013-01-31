using UnityEngine;
using System.Collections;

public class GridCreator : MonoBehaviour {
	public GameObject prefab;
	public static int height = 40;
	public static int width = 40;
	public float startingProb = 0.5f;
	private GameObject[,] gridObjects = new GameObject[height,width];
	private int[,] stateMatrix = new int[height,width];
	
	void Start () {
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
			 	GameObject obj = (GameObject) Instantiate(prefab, new Vector3(i, 0f, -j), Quaternion.identity);
				obj.transform.parent = this.transform;
				obj.transform.renderer.material.color = Color.white;
				gridObjects[i,j] = obj;
				stateMatrix[i,j] = 0;
			}
		}
		this.transform.position += new Vector3(0,0,40);
		CreateProbabilityMatrix();
	}
	
	void Update() {
		if(Input.GetKeyDown(KeyCode.Alpha1)) {
			IterateGeneration(1);	
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			IterateGeneration(2);	
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			IterateGeneration(3);	
		} else if (Input.GetKeyDown(KeyCode.Alpha4)) {
			IterateGeneration(4);	
		}
	}

	private float[,] probMatrix = new float[height,width];
	public bool startingOffset = true;
	public int numStartingPoints = 5; 
	void CreateProbabilityMatrix () {
		Vector3[] startingValues = new Vector3[numStartingPoints];
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
			 	probMatrix[i,j] = startingProb;
			}
		}
		if (startingOffset) {
			for (int x = 0; x < startingValues.Length; x++) {
				startingValues[x] = new Vector3(Random.Range(0, height - 1),0, Random.Range(0, width  - 1));
			}
			
			bool isFarEnoughApart = false;
			int hadEnough = 50;
			int counter = 0;
			while (!isFarEnoughApart) {
				if (counter > hadEnough) {
					Debug.Log("BROKEN");
					break;	
				}
				isFarEnoughApart = true;
				for (int i = 0; i < startingValues.Length; i++) {
					Vector3 baseValue = startingValues[i];
					foreach (Vector3 otherValues in startingValues) {
						if (otherValues == baseValue) {
							continue;	
						} 
						if (Vector3.Distance(baseValue, otherValues) < 8f) {
							if (otherValues.x > baseValue.x && (int) baseValue.x != 0) {
								baseValue.x -= 1;
								isFarEnoughApart = false;
							} else if (otherValues.x <= baseValue.x && (int) baseValue.x != height - 1) {
								baseValue.x += 1;	
							}
							if (otherValues.z > baseValue.z && (int) baseValue.z != 0) {
								baseValue.z -= 1;
							} else if (otherValues.z <= baseValue.z && (int) baseValue.z != width - 1) {
								baseValue.z += 1;	
							}
							isFarEnoughApart = false;
						}
					}
				}
				counter++;
			}	
			
			for (int x = 0; x < startingValues.Length; x++) {
				Debug.Log(startingValues[x]);
				int randomX = (int) startingValues[x].x;
				int randomY = (int) startingValues[x].z;
				probMatrix[randomX, randomY] += 0.5f;
				gridObjects[randomX,randomY].transform.renderer.material.color = Color.red;
				foreach (Collider hit in Physics.OverlapSphere(gridObjects[randomX, randomY].transform.position, 10f)) {
					if (hit.collider.gameObject == gridObjects[randomX,randomY]) {
						
					} else if (Vector3.Distance(hit.transform.position, gridObjects[randomX, randomY].transform.position) < 2) {
						gridObjects[(int)hit.transform.localPosition.x, -(int)hit.transform.localPosition.z].transform.renderer.material.color = Color.green;
						probMatrix[(int)hit.transform.localPosition.x, -(int)hit.transform.localPosition.z] += 0.5f;
					} else if (Vector3.Distance(hit.transform.position, gridObjects[randomX, randomY].transform.position) < 5) {
						gridObjects[(int)hit.transform.localPosition.x, -(int)hit.transform.localPosition.z].transform.renderer.material.color = Color.magenta;
						probMatrix[(int)hit.transform.localPosition.x, -(int)hit.transform.localPosition.z] += 0.4f;
					} else if (Vector3.Distance(hit.transform.position, gridObjects[randomX, randomY].transform.position) < 8) {
						gridObjects[(int)hit.transform.localPosition.x, -(int)hit.transform.localPosition.z].transform.renderer.material.color = Color.yellow;
						probMatrix[(int)hit.transform.localPosition.x, -(int)hit.transform.localPosition.z] += 0.2f;
					}
				}
			}
		}
		PopulateWalls();
	}
	
	void PopulateWalls() {
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
			 	if (Random.value > probMatrix[i,j]) {
					gridObjects[i,j].transform.renderer.material.color = Color.black;
					stateMatrix[i,j] = 1;
				}
			}
		}
	}
	
	void IterateGeneration(int rule) {
		if (rule == 1) { 
			for (int i = 0; i < height; i++) {
				for (int j = 0; j < width; j++) {
				 	if (GetNeighborScore(i,j) >= 5) {
						gridObjects[i,j].transform.renderer.material.color = Color.black;
					} else {
						gridObjects[i,j].transform.renderer.material.color = Color.white;	
					}
				}
			}
		} else if (rule == 2) {
			for (int i = 0; i < height; i++) {
				for (int j = 0; j < width; j++) {
					int neighborScore = GetNeighborScore(i,j);
					bool isBlack = gridObjects[i,j].transform.renderer.material.color == Color.black;
				 	if ((neighborScore > 5 && !isBlack) || (neighborScore > 3 && isBlack)) {
						gridObjects[i,j].transform.renderer.material.color = Color.black;
					} else {
						gridObjects[i,j].transform.renderer.material.color = Color.white;	
					}
				}
			}
		} else if (rule == 3) {
			for (int i = 0; i < height; i++) {
				for (int j = 0; j < width; j++) {
					int neighborScore = GetNeighborScore(i,j);
					bool isBlack = gridObjects[i,j].transform.renderer.material.color == Color.black;
				 	if ((neighborScore >= 5 && !isBlack) || (neighborScore >= 5 && isBlack)) {
						gridObjects[i,j].transform.renderer.material.color = Color.black;
					} else {
						gridObjects[i,j].transform.renderer.material.color = Color.white;	
					}
				}
			}
		 } else if (rule == 4) {
			for (int i = 0; i < height; i++) {
				for (int j = 0; j < width; j++) {
					int neighborScore = GetNeighborScore(i,j);
					bool isBlack = gridObjects[i,j].transform.renderer.material.color == Color.black;
				 	if ((neighborScore >= Random.Range(1,5) && !isBlack) || (neighborScore >= Random.Range(4,7) && isBlack)) {
						gridObjects[i,j].transform.renderer.material.color = Color.black;
					} else {
						gridObjects[i,j].transform.renderer.material.color = Color.white;	
					}
				}
			}
		}
		UpdateStateMatrix();
	}
	
	void UpdateStateMatrix() {
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
			 	if (gridObjects[i,j].transform.renderer.material.color == Color.black) {
					stateMatrix[i,j] = 1;
				} else {
					stateMatrix[i,j] = 0;
				}
			}
		}
	}
	
	int GetNeighborScore(int i, int j) {
		int score = stateMatrix[i,j];
		if (i != 0)
			score += stateMatrix[i-1,j];
		if (j != 0)
			score += stateMatrix[i,j-1];
		if (i != height - 1)
			score += stateMatrix[i+1,j];
		if (j != width - 1)
			score += stateMatrix[i,j+1];
		if (i != 0 && j != 0)
			score += stateMatrix[i-1,j-1];
		if (i != 0 && j != width - 1)
			score += stateMatrix[i-1,j+1];
		if (i != height - 1 && j != 0)
			score += stateMatrix[i+1,j-1];
		if (i != height - 1 && j != width - 1)
			score += stateMatrix[i+1,j+1];
		return score;
	}
}
