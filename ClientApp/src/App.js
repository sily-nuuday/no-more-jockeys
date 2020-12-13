import React, { Component } from 'react'
import HomeScreen from './HomeScreen.js'
import Game from './Game.js'
import { HubConnectionBuilder } from '@microsoft/signalr'
import './App.css'

class App extends Component {
    constructor(props) {
        super(props);

        this.state = { showGame: false, connectionLoaded: false };

        this.setConnectionLoaded = this.setConnectionLoaded.bind(this);
        this.showGameList = this.showGameList.bind(this);
        this.showGame = this.showGame.bind(this);

        this.connection = new HubConnectionBuilder().withUrl('/games').build();
        this.connection.start().then(this.setConnectionLoaded);
    }

    setConnectionLoaded() {
        this.setState({
            connectionLoaded: true
        });
    }

    showGameList() {
        this.setState({
            showGame: false
        });
    }

    showGame() {
        this.setState({
            showGame: true
        });
    }

    render() {
        var element;
        if (!this.state.connectionLoaded) {
            element = <div>Loading...</div>;
        } else if (this.state.showGame) {
            element = <Game connection={this.connection}
                            gameCode={this.state.gameCode}
                            showGameListHandler={this.showGameList} />;
        } else {
            element = <HomeScreen connection={this.connection}
                                  showGameHandler={this.showGame} />;
        }

        return element;
    }
}

export default App