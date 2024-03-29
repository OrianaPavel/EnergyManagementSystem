import React from 'react';
import APIResponseErrorMessage from "../commons/errorhandling/api-response-error-message";
import { Button, Card, CardHeader, Col, Modal, ModalBody, ModalHeader, Row } from 'reactstrap';
import DeviceForm from "./components/device-form";  
import * as API_DEVICES from "./api/device-api";   
import DeviceTable from "./components/device-table";  
class DeviceContainer extends React.Component {

    constructor(props) {
        super(props);
        this.toggleForm = this.toggleForm.bind(this);
        this.reload = this.reload.bind(this);
        this.reloadAfterModify = this.reloadAfterModify.bind(this);
        this.state = {
            selected: false,
            tableData: [],
            isLoaded: false,
            errorStatus: 0,
            error: null
        };
    }

    componentDidMount() {
        this.fetchDevices();
    }

    fetchDevices() {
        const userId = this.props.match.params.userId; 

        return API_DEVICES.getDevicesByUserId(userId, (result, status, err) => {
            if (result !== null && status === 200) {
                this.setState({
                    tableData: result,
                    isLoaded: true
                });
            } else {
                this.setState(({
                    errorStatus: status,
                    error: err
                }));
            }
        });
    }

    

    toggleForm() {
        this.setState({selected: !this.state.selected});
    }

    reload() {
        this.setState({
            isLoaded: false
        });
        this.toggleForm();
        this.fetchDevices();
    }

    reloadAfterModify() {
        this.setState({
            isLoaded: false
        });
        //this.toggleForm();
        this.fetchDevices();
    }

    render() {
        const currentUserRole = localStorage.getItem('role');
        const currentUserId = localStorage.getItem('userId');
        const userId = this.props.match.params.userId; 
        if (currentUserRole !== '1' && userId !== currentUserId) {  
            console.log("currentUserId === " + currentUserId + " userId ==  " + userId + "EXPR VAL = " + (currentUserRole !== '1' || userId !== currentUserId));
            return <div>Access Denied. </div>
        }
        return (
            <div>
                <CardHeader>
                    <strong> Device Management </strong>
                </CardHeader>
                <Card>
                    <br/>
                    <Row>
                        <Col sm={{size: '8', offset: 1}}>
                            <Button color="primary" onClick={this.toggleForm}>Add Device </Button>
                        </Col>
                    </Row>
                    <br/>
                    <Row>
                        <Col sm={{size: '8', offset: 1}}>
                            {this.state.isLoaded && <DeviceTable tableData = {this.state.tableData}
                                                                 userId={this.props.match.params.userId}  
                                                                 reloadHandler={this.reloadAfterModify}                       
                            />}
                            {this.state.errorStatus > 0 && <APIResponseErrorMessage
                                                            errorStatus={this.state.errorStatus}
                                                            error={this.state.error}
                                                        />   }
                        </Col>
                    </Row>
                </Card>

                <Modal isOpen={this.state.selected} toggle={this.toggleForm}
                       className={this.props.className} size="lg">
                    <ModalHeader toggle={this.toggleForm}> Add Device: </ModalHeader>
                    <ModalBody>
                        <DeviceForm reloadHandler={this.reload}
                                    userId={this.props.match.params.userId} 
                        />
                    </ModalBody>
                </Modal>
            </div>
        );
    }
}

export default DeviceContainer;
