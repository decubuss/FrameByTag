using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FrameSequenceController : MonoBehaviour
{
    public FrameSequenceController instance;
    public List<Frame> FrameSequence;
    private Frame CurrentFrame;

    private CameraSetter CameraSetter;
    private ObjectsPlacementController OPController;
    // Start is called before the first frame update
    void Start()
    {
        CameraSetter = FindObjectOfType<CameraSetter>();
        OPController = FindObjectOfType<ObjectsPlacementController>();
        FrameSequence = new List<Frame>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateFrame()
    {
        Frame newFrame = new Frame(CameraSetter.GetShotParameters(), OPController._lastExecutedElements, OPController._lastExecutedTags, FrameDescription.RawFrameInput);
        FrameSequence.Add(newFrame);
    }
    public void ReadFrame(int index)
    {
        Frame frame = FrameSequence[index];
        OPController.PrepareScene(frame.DescriptionTags, frame.ShotElements);
        CameraSetter.UpdateCameraTransform(frame.ShotParameters);
        CurrentFrame = frame;
    }
    public void UpdateFrame(int index)
    {
        //FrameSequence.FindIndex(ind => ind.Equals(CurrentFrame))
        FrameSequence[index] = new Frame();
    }
    public void DeleteFrame(int index)
    {
        FrameSequence.Remove(FrameSequence[index]);
    }
}
