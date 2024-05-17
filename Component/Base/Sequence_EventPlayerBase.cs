using Threeyes.Core;
namespace Threeyes.EventPlayer
{
    public interface ISequence_EventPlayer : ISequence
    {
        /// <summary>
        /// 功能：Hierarchy中返回子EP的序号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int FindIndexForDataEditor(EventPlayer data);
    }

    public class Sequence_EventPlayerBase<TEventPlayer> : SequenceForCompBase<TEventPlayer>, ISequence_EventPlayer
        where TEventPlayer : EventPlayer
    {
        #region Inner Method
        protected override void SetDataValid(TEventPlayer data)
        {
            data.IsActive = true;
        }
        protected override bool IsDataVaild(TEventPlayer data)
        {
            return data.IsActive;
        }
        protected override void SetDataFunc(TEventPlayer data, int index)
        {
            SetData_CustomFunc(data, index);
            base.SetDataFunc(data, index);
        }
        protected override void ResetDataFunc(TEventPlayer data, int index)
        {
            ResetData_CustomFunc(data, index);
            base.ResetDataFunc(data, index);
        }

        protected virtual void SetData_CustomFunc(TEventPlayer data, int index)
        {
            data.Play();
        }
        protected virtual void ResetData_CustomFunc(TEventPlayer data, int index)
        {
            data.Stop();
        }
        #endregion

        public int FindIndexForDataEditor(EventPlayer data)
        {
            if (IsLoadChildOnAwake && data is TEventPlayer eventPlayerReal)
            {
                return GetComponentsFromChild().IndexOf(eventPlayerReal);
            }
            return -1;
        }
    }
}