using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IScanerTarget
{
    Vector3 Center { get; }
    Transform transform { get; }
}
