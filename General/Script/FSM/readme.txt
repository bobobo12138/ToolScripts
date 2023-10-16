基于接口的状态机
目的在于解决状态机State间通信问题

假设你有一个obj名叫Tower

你需要自定义接口IGFSM_Tower:IGFSM			--方便主类中获取参数
你需要自定义状态TowerState:State_FSM<IGFSM_Tower>	--由状态继承，状态中可通过user.接口方法()获取所需参数
最后在主类中继承IGFSM_Tower


之后像常规状态机一样运作即可