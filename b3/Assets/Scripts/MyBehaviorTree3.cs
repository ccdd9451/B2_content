using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using System;

public class MyBehaviorTree3 : MonoBehaviour {

    private BehaviorAgent behaviorAgent;

#region Characters
    public GameObject hero;
    public GameObject[] supporter = new GameObject[0];

    public int friendGroupNum = 3;
    #endregion


    protected Node BuildTreeRoot()
    {
        IList<Node> allEvents = new List<Node>();
        int sl = supporter.Length;
        for (int i = 0; i < sl - friendGroupNum * 2; i++)
        {
            allEvents.Add(
                TreeUtils.TreeNodeTrace(
                GetComponent<SupporterBehavior>().BehaviorSingle(supporter[i]),
                "Individual Enter", false));
        }

        for (int i = 0; i < friendGroupNum * 2; i += 2)
        {
            allEvents.Add(GetComponent<SupporterBehavior>()
                .BehaviorFriend(supporter[sl - i - 1], supporter[sl - i - 2]));
        }
        allEvents.Add( GetComponent<HeroBehavior>().GetBehavior(hero) );
        Node roaming = new SelectorParallel(allEvents.ToArray());

        return new DecoratorLoop(roaming);
    }

    void Start()
    {
        SetupScene();
        supporter = GameObject.FindGameObjectsWithTag("Daniel");


        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();


    }

    void SetupScene()
    {
        GameObject wallet = GameObject.Find("Wallet");
        GameObject hero = GameObject.Find("LeadDaniel");
        GameObject pocket = hero.GetComponent<BodyMecanim>().Reflist.pocket;

        wallet.GetComponent<Rigidbody>().isKinematic = true;
        wallet.transform.parent = pocket.transform;
        wallet.transform.localPosition = Vector3.zero;
        wallet.transform.localRotation = Quaternion.identity;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
