using System;
using System.IO;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Math;

namespace TerrainCalculator
{
	public class RiverMenu : MonoBehaviour
	{
		bool initialized = false;
		string settingsFilePath;
		Camera mainCam;
		Rect windowRect = new Rect(30, 400, 120, 45);
		int menuX = 30;
		int menuY = 400;
		int menuWidth = 120;
		int menuHeight = 45;
		string helpText = "";
		bool active = false;
		bool editMode = false;
		bool adjustScale = false;
		bool adjustPos = false;
		Vector3 hitpos = Vector3.zero;
		Vector3 cursorScale = new Vector3(96, 48, 96);
		Vector3 storedMousePosition = Vector3.zero;
		Vector3 storedScale = Vector3.zero;
		int selection = -1;
		Material cursorMaterial;
		Material nodeMaterial;
		Material shapeMaterial;
		GameObject cursorNode;
		Mesh nodeMesh;
		void Initialize()
		{
			if (!initialized)
			{
				mainCam = Camera.main;
				UnityEngine.Random.InitState(Time.frameCount);
				cursorNode = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				cursorMaterial = new Material(Shader.Find("Sprites/Default"));
				cursorMaterial.color = new Color(1, 1, 0, 0.75f);
				cursorNode.GetComponent<Renderer>().material = cursorMaterial;
				cursorNode.transform.position = Vector3.up * -200;

				nodeMesh = cursorNode.GetComponent<MeshFilter>().mesh;
				nodeMaterial = new Material(Shader.Find("Sprites/Default"));
				nodeMaterial.color = new Color(0, 1, 1, 0.5f);

				shapeMaterial = new Material(Shader.Find("GUI/Text Shader")); //thanx to 'ronyx69's shader list
				shapeMaterial.color = new Color(0, 0, 1, 0.5f);
				initialized = true;
			}
		}
		void Start()
		{
			settingsFilePath = Application.dataPath + "/TRTSettings.txt";
			settingsFilePath = settingsFilePath.Replace(@"/", @"\");
			if (File.Exists(settingsFilePath))
			{
				string data = File.ReadAllText(settingsFilePath);
				string[] lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
				if (lines.Length > 0)
				{
					string[] menuPosition = lines[0].Split(':');
					string[] pos = menuPosition[1].Split(',');
					menuX = int.Parse(pos[0]);
					if (menuX + menuWidth > Screen.width)
					{
						menuX = Screen.width - menuWidth;
					}
					if (menuX < 0)
					{
						menuX = 0;
					}
					menuY = int.Parse(pos[1]);
					if (menuY + menuHeight > Screen.height)
					{
						menuY = Screen.height - menuHeight;
					}
					if (menuY < 0)
					{
						menuY = 0;
					}
				}
				if (lines.Length > 2)
				{
					Tool.loadMapO = lines[1].Split(':')[1] == "True";
					Tool.loadGameO = lines[2].Split(':')[1] == "True";
				}
				if (lines.Length > 3)
				{
					Tool.clearAfterCarve = lines[3].Split(':')[1] == "True";
				}
			}
			windowRect = new Rect(menuX, menuY, menuWidth, menuHeight);
		}
		void Update()
		{
			if (active && initialized)
			{
				Singleton<SimulationManager>.instance.SimulationPaused = true;
				if (Tool.updateHeight)
				{
					Tool.UpdateHeight();
					Tool.updateHeight = false;
				}
				if (Input.GetMouseButtonDown(0) && UIView.IsInsideUI() && !OnMenu())
				{ //abort!!
					active = false;
					editMode = false;
					adjustPos = false;
					adjustScale = false;
					selection = -1;
					cursorNode.transform.position = Vector3.up * -200f; //hide
					return;
				}
				Edit();
				RenderNodes();
			}
		}
		void Edit()
		{
			if (editMode)
			{
				if (Input.GetKeyDown(KeyCode.LeftShift))
				{ //adjust scale/depth
					storedMousePosition = Input.mousePosition;
					storedScale = cursorScale;
					adjustScale = true;
				}
				if (Input.GetKeyUp(KeyCode.LeftShift))
				{
					adjustScale = false;
				}
				if (Input.GetMouseButtonUp(0))
				{
					adjustPos = false;
				}
				if (Input.GetMouseButtonDown(1))
				{
					if (selection < 0)
					{ //back
						editMode = false;
					}
					else
					{ //delete node
						Tool.nodes[selection].RemoveNode();
						selection = -1;
					}
				}
				if (adjustPos)
				{ //can only be called if selected
					Tool.nodes[selection].SetPosition(hitpos);
				}
				else if (adjustScale)
				{
					Vector3 mousePosition = Input.mousePosition;
					float addWidth = mousePosition.x - storedMousePosition.x;
					float addDepth = mousePosition.y - storedMousePosition.y;
					float x = storedScale.x;
					float y = storedScale.y;
					float z = storedScale.z;
					float maxDepth = Tool.terrain.SampleTerrainHeight(cursorNode.transform.position.x, cursorNode.transform.position.z);
					x = Mathf.Clamp(x + addWidth, 16f, 1024f);
					y = Mathf.Clamp(y + addDepth, 0, maxDepth);
					z = Mathf.Clamp(z + addWidth, 16f, 1024f);
					x = Mathf.Round(x);
					y = Mathf.Round(y);
					z = Mathf.Round(z);
					cursorScale = new Vector3(x, y, z);
					helpText = "Depth " + cursorScale.y.ToString() + "\r\nWidth " + cursorScale.x.ToString();
				}
				if (selection >= 0)
				{ //a node is selected
					cursorNode.transform.position = Tool.nodes[selection].position;
					Tool.nodes[selection].SetScale(cursorScale); //cursor already has scale of node. this is for adjusting
				}
				if (!adjustScale)
				{
					if (RaycastTerrain())
					{
						if (!adjustPos)
						{
							selection = -1;
							for (int s = 0; s < Tool.nodes.Count; s++)
							{
								float dist = (hitpos - Tool.nodes[s].position).sqrMagnitude; //it's cheaper 
								if (dist * 2 < Tool.nodes[s].scale.x * Tool.nodes[s].scale.x)
								{ //close enough?
									selection = s; //selection for next round; could be the same...whatever
									cursorScale = Tool.nodes[selection].scale;
									helpText = "Click left and drag to move\r\nLeft shift and drag to adjust\r\nClick right to delete";
								}
							}

						}
						if (selection < 0)
						{ //nothing selected
							cursorNode.transform.position = hitpos;
							if (Tool.nodes.Count < 3)
							{
								helpText = "Click left to add node\r\nClick right for menu";
							}
							if (Input.GetMouseButtonDown(0))
							{
								Tool.nodes.Add(new RiverNode(hitpos, cursorScale));
							}
						}
						else
						{
							if (Input.GetMouseButtonDown(0))
							{
								storedMousePosition = Input.mousePosition;
								adjustPos = true;
							}
						}
					}
					else
					{
						helpText = "";
						cursorNode.transform.position = Vector3.up * -200f; //hide again
					}
				}
				cursorNode.transform.localScale = cursorScale;
			}
			else
			{
				if (Input.GetMouseButtonDown(1))
				{
					active = false;
				}
				cursorNode.transform.position = Vector3.up * -200f; //just hide
			}
		}
		void RenderNodes()
		{
			if (Tool.nodes.Count > 0)
			{
				for (int p = 0; p < Tool.nodes.Count; p++)
				{
					Matrix4x4 matrix = Matrix4x4.TRS(Tool.nodes[p].position, Quaternion.identity, Tool.nodes[p].scale);
					Graphics.DrawMesh(nodeMesh, matrix, nodeMaterial, 0); //draw nodes
					if (Tool.nodes[p].mesh != null)
					{
						matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
						Graphics.DrawMesh(Tool.nodes[p].mesh, matrix, shapeMaterial, 0); //draw part river mesh from node
					}
				}
			}
		}
		bool RaycastTerrain()
		{
			Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
			Vector3 origin = mouseRay.origin;
			Vector3 vector = origin + (mouseRay.direction.normalized * mainCam.farClipPlane);
			Segment3 ray = new Segment3(origin, vector); //using colossal math just for this 
			if (Singleton<TerrainManager>.instance.RayCast(ray, out hitpos))
			{
				if (Mathf.Abs(hitpos.x) < 8632f && Mathf.Abs(hitpos.z) < 8632f)
				{
					return true;
				}
			}
			return false;
		}
		void OnGUI()
		{
			if ((Tool.loadMap && Tool.loadMapO) || (Tool.loadGame && Tool.loadGameO))
			{
				if (editMode)
				{
					GUI.color = Color.black;
					GUI.Label(new Rect(Input.mousePosition.x + 20f, Screen.height - Input.mousePosition.y - 20f, Screen.width, Screen.height), helpText);
					return;
				}
				if (active && Tool.nodes.Count > 0)
				{
					windowRect.width = 120;
					windowRect.height = 145;
				}
				else
				{
					windowRect.width = 120;
					windowRect.height = 45;
				}
				GUI.color = Color.white;
				windowRect = GUI.Window(14142, windowRect, Menu, "TerrainRiverTool");
			}
		}
		void Menu(int windowID)
		{
			if (!active || Tool.nodes.Count == 0)
			{
				if (Tool.nodes.Count == 0)
				{
					if (GUI.Button(new Rect(5, 19, 110, 20), "CreateRiver"))
					{
						Singleton<ToolsModifierControl>.instance.CloseEverything();
						Initialize();
						Tool.nodes.Clear();
						active = true;
						editMode = true;
					}
				}
				else
				{
					if (GUI.Button(new Rect(5, 19, 110, 20), "Activate"))
					{
						Singleton<ToolsModifierControl>.instance.CloseEverything();
						active = true;
					}
				}
			}
			else
			{
				if (GUI.Button(new Rect(5, 17, 110, 20), "Edit"))
				{
					editMode = true;
				}
				if (GUI.Button(new Rect(5, 37, 110, 20), "LerpHeight"))
				{
					Tool.InterpolateH();
				}
				if (GUI.Button(new Rect(5, 57, 110, 20), "LerpWidth"))
				{
					Tool.InterpolateW();
				}
				if (GUI.Button(new Rect(5, 77, 110, 20), "LerpDepth"))
				{
					Tool.InterpolateD();
				}
				if (GUI.Button(new Rect(5, 97, 110, 20), "Reset"))
				{
					Tool.nodes.Clear();
					active = true;
					editMode = true;
				}
				if (Tool.nodes.Count > 1)
				{
					if (GUI.Button(new Rect(5, 117, 110, 20), "Carve"))
					{
						Tool.CarveRiver();
						active = false;
						editMode = false;
					}
				}
				else
				{
					if (GUI.Button(new Rect(5, 117, 110, 20), "TryRiver!"))
					{
						Tool.TryRiver();
					}
				}
			}
			GUI.DragWindow(new Rect(0, 0, 10000, 20));
		}
		bool OnMenu()
		{
			Vector3 mPos = Input.mousePosition;
			float x = mPos.x;
			float y = Screen.height - mPos.y;
			if (x > windowRect.x && x < windowRect.x + windowRect.width && y > windowRect.y && y < windowRect.y + windowRect.height)
			{
				return true;
			}
			return false;
		}
		void OnDestroy()
		{
			string data =
				"MenuPosition:" + windowRect.x.ToString() + "," + windowRect.y.ToString() + "\r\n" +
				"Use in TerrainEditor:" + Tool.loadMapO.ToString() + "\r\n" +
				"Use in Game:" + Tool.loadGameO.ToString() + "\r\n" +
				"Clear after carve:" + Tool.clearAfterCarve.ToString();
			File.WriteAllText(settingsFilePath, data);
		}
	}
}
