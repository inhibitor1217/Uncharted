
/* ----------------------------------------------------- */

// Random ID Generator
var LEGAL_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
var ID_LENGTH = 8;
var IDGen = () => {
	var ret = "";
	for(var i = 0; i < ID_LENGTH; i++) {
		ret += LEGAL_CHARACTERS.charAt(Math.floor(Math.random() * LEGAL_CHARACTERS.length));
	}
	return ret;
};

/* ----------------------------------------------------- */

var _ESP_ = 0.001;
/* Task Heap Implementation */
var TaskHeap = () => {

	var self = {};

	self._data = [];
	self._MAX_DATA_ = 1024;
	self._front = 0;
	self._rear = 0;
	
	self._index = (idx) => {
		return (idx + self._MAX_DATA_) % self._MAX_DATA_;
	}

	self.Enqueue = (data) => {
		if(self.Size() >= self._MAX_DATA_ - 1)
			console.log('WARNING: Task Heap is full.');
		else {
			self._data[self._rear] = data;
			self._rear = self._index(self._rear + 1);
		}
	};

	self.Dequeue = () => {
		if(self.Empty()) {
			console.log('ERROR: Task Heap is empty.');
			return null;
		}
		else {
			var ret = self._data[self._front];
			self._front = self._index(self._front + 1);
			return ret;
		}
	};

	self.Peek = () => {
		return self._data[0];
	};

	self.Empty = () => {
		return (self.Size() == 0);
	};

	self.Size = () => {
		return self._index(self._rear - self._front);
	}

	return self;

};

/* ----------------------------------------------------- */

var io = require('socket.io').listen(2000);
io.set('log level', 0); // DISABLE DEBUG PRINT

var SOCKET_LIST = {};

var tickCount = 0;
var TICKS_PER_SECOND = 5.0;

var heap = TaskHeap();
 
io.sockets.on('connection', (socket) => {

	while(socket._id == null || socket._id in SOCKET_LIST) {
		socket._id = IDGen();
	}

	SOCKET_LIST[socket._id] = socket;
	socket.serverTime = new Date().getTime();

	console.log('connection: USER CONNECTED: ' + socket._id);

	socket.emit('Configure', {
		'id': socket._id
	});

    socket.on('Task', (data) => {
    	var parsedData = JSON.parse(data);
    	heap.Enqueue(parsedData);
    });

    socket.on('disconnect', () => {
    	console.log('disconnect: USER DISCONNECTED: ' + socket._id);
    	delete SOCKET_LIST[socket._id];
    });
 
});

/* ----------------------------------------------------- */

setInterval(() => {

	var updated = false;
	var buffer = {
		chat: []
	};

	while(!heap.Empty()) {
		updated = true;
		var task = heap.Dequeue();
		if(task.type == 'c') {
			buffer.chat.push({
				'id': task.id,
				'msg': task.payload.msg
			});
		}
	}

	if(updated) {
		for(var i in SOCKET_LIST)
			SOCKET_LIST[i].emit('Update', buffer);
	}
	
	tickCount++;

}, 1000.0 / TICKS_PER_SECOND);