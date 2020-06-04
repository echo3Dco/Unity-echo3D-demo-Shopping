/**************************************************************************
* Copyright (C) echoAR, Inc. 2018-2020.                                   *
* echoAR, Inc. proprietary and confidential.                              *
*                                                                         *
* Use subject to the terms of the Terms of Service available at           *
* https://www.echoar.xyz/terms, or another agreement                      *
* between echoAR, Inc. and you, your company or other organization.       *
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBehaviour : MonoBehaviour {
    [HideInInspector]
    public Entry entry;

    /// <summary>
    /// EXAMPLE BEHAVIOUR
    /// Queries the database and names the object based on the result.
    /// </summary>

    public class Data {
      public string description;
      public string price;
      public string url;

      public Data(string description, string price, string url) {
        this.description = description;
        this.price = price;
        this.url = url;
      }
    }

    GameObject product = null;
    GameObject scene = null;
    GameObject model = null;
    Data data = null;

    // Use this for initialization
    void Start() {
        this.gameObject.tag = "product";
      // Add RemoteTransformations script to object and set its entry
      this.gameObject.AddComponent<RemoteTransformations>().entry = entry;

      // Attach a CameraController script to allow rotation with Mouse and moving with Arrow Keys
      if (Camera.main.gameObject.GetComponent<CameraController>() == null) {
        Camera.main.gameObject.AddComponent(typeof(CameraController));
        Camera.main.GetComponent<CameraController>().enabled = false; // Disable the controller until everyting is loaded
        }
      
      // Qurey additional data to get the name
      string value = "";
      if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("name", out value)) {
        // Set name
        this.gameObject.name = value;
      }

      // Create cart text
      if (this.gameObject.name == "cart") {
        string totalString = "";
        if (entry.getAdditionalData().TryGetValue("total", out totalString)) {
          GameObject totalText = new GameObject();
          totalText.name = "cartTotal";
          totalText.transform.position = new Vector3(-13, 2, 0);
          TextMesh totalTextMesh = totalText.AddComponent<TextMesh>();
          totalTextMesh.text = "$" + totalString;
          totalTextMesh.fontSize = 12;
          totalTextMesh.anchor = TextAnchor.MiddleCenter;
          totalTextMesh.alignment = TextAlignment.Center;
        }
        string itemsString = "";
        if (entry.getAdditionalData().TryGetValue("items", out itemsString)) {
          GameObject itemsText = new GameObject();
          itemsText.name = "cartItems";
          itemsText.transform.position = new Vector3(-13, -3, 0);
          TextMesh itemsTextMesh = itemsText.AddComponent<TextMesh>();
          itemsTextMesh.text = itemsString + " item(s) in cart";
          itemsTextMesh.fontSize = 12;
          itemsTextMesh.anchor = TextAnchor.MiddleCenter;
          itemsTextMesh.alignment = TextAlignment.Center;
        }
      }
    }

    // Update is called once per frame
    void Update() {
        // Enable movement of the camera
        if (this.gameObject.name.Equals("Room.glb") && !Camera.main.GetComponent<CameraController>().enabled)
        {
            Camera.main.GetComponent< CameraController >().enabled = true;
            GameObject.Find("Canvas").SetActive(false);// Disable the Loading text
        }
      if (this.gameObject.name == "cart") return;

      if (product == null) {
        product = this.gameObject;
      }
      if (product != null && scene == null && product.transform.childCount > 0) {
        scene = product.transform.GetChild(0).gameObject;
      }
      if (scene != null && model == null && scene.transform.childCount > 0) {
        model = scene.transform.GetChild(0).gameObject;
        model.AddComponent<MeshCollider>();

        // display product name below model
        GameObject nameText = new GameObject();
        nameText.name = "name" + product.name;
        nameText.transform.position = model.transform.position + new Vector3(0, -2, 0);
        TextMesh nameTextMesh = nameText.AddComponent<TextMesh>();
        nameTextMesh.text = product.name;
        nameTextMesh.fontSize = 15;
        nameTextMesh.anchor = TextAnchor.MiddleCenter;
        nameTextMesh.alignment = TextAlignment.Center;
        nameText.AddComponent(typeof(FollowCamera));

        if (entry.getAdditionalData() != null) {
          // get description from metadata and display it
          string description = "";
          if (entry.getAdditionalData().TryGetValue("description", out description)) {
              GameObject descriptionText = new GameObject();
              descriptionText.name = "description" + model.name;
              descriptionText.transform.position = model.transform.position + new Vector3(0, -4, 0);
              TextMesh descriptionTextMesh = descriptionText.AddComponent<TextMesh>();
              descriptionTextMesh.text = description;
              descriptionTextMesh.fontSize = 12;
              descriptionTextMesh.anchor = TextAnchor.MiddleCenter;
              descriptionTextMesh.alignment = TextAlignment.Center;
              descriptionText.AddComponent(typeof(FollowCamera));
                }

          // get price from metadata and display it
          string priceString = "";
          if (entry.getAdditionalData().TryGetValue("price", out priceString)) {
            // display text above model
            GameObject priceText = new GameObject();
            priceText.name = "price" + model.name;
            priceText.transform.position = model.transform.position + new Vector3(0, 8, 0);
            TextMesh priceTextMesh = priceText.AddComponent<TextMesh>();
            priceTextMesh.text = "$" + priceString;
            priceTextMesh.fontSize = 20;
            priceTextMesh.anchor = TextAnchor.MiddleCenter;
            priceTextMesh.alignment = TextAlignment.Center;
            priceText.AddComponent(typeof(FollowCamera));
                }

          // get url from metadata
          string itemURL = "";
          if (entry.getAdditionalData().TryGetValue("url", out itemURL)) {}

          // create Data object
          data = new Data(description, priceString, itemURL);
        }
      }

      // click listener for each model to redirect user to item page and add item to cart
      if (Input.GetMouseButtonDown(0)) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
          if (hit.transform && model.name == hit.transform.gameObject.name) {
            // redirect to item page
            if (data.url != "") {
              Application.OpenURL(data.url);
            }

            // simulate adding an item to cart
            GameObject totalCartGameObject = GameObject.Find("cartTotal");
            GameObject itemsCartGameObject = GameObject.Find("cartItems");

            if (totalCartGameObject != null && itemsCartGameObject != null) {
              string temp = totalCartGameObject.GetComponent<TextMesh>().text.Substring(1);
              int newCartTotal = int.Parse(temp) + int.Parse(data.price);
              totalCartGameObject.GetComponent<TextMesh>().text = "$" + newCartTotal;

              temp = itemsCartGameObject.GetComponent<TextMesh>().text.Substring(0, 1);
              int newCartItems = int.Parse(temp) + 1;
              itemsCartGameObject.GetComponent<TextMesh>().text = newCartItems + " item(s) in cart";
            }
          }
        }
      }
    }
}
