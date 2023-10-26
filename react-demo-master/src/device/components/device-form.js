import React from 'react';
import validate from "./validators/device-validators";  // Adjust to your device validator's path
import Button from "react-bootstrap/Button";
import * as API_DEVICES from "../api/device-api";  
import APIResponseErrorMessage from "../../commons/errorhandling/api-response-error-message";
import {Col, Row} from "reactstrap";
import { FormGroup, Input, Label } from 'reactstrap';

class DeviceForm extends React.Component {

    constructor(props) {
        super(props);
        this.toggleForm = this.toggleForm.bind(this);
        this.reloadHandler = this.props.reloadHandler;

        this.state = {
            errorStatus: 0,
            error: null,
            formIsValid: false,

            formControls: {
                description: {
                    value: '',
                    placeholder: 'Description...',
                    valid: false,
                    touched: false,
                    validationRules: {
                        minLength: 5,
                        isRequired: true
                    }
                },
                address: {
                    value: '',
                    placeholder: 'Address...',
                    valid: false,
                    touched: false,
                    validationRules: {
                        minLength: 5,
                        isRequired: true
                    }
                },
                maximumHourlyEnergyConsumption: {
                    value: '',
                    placeholder: 'Max Hourly Energy Consumption...',
                    valid: false,
                    touched: false,
                    validationRules: {
                        isRequired: true,
                        isInt: true
                    }
                },
            }
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    toggleForm() {
        this.setState({collapseForm: !this.state.collapseForm});
    }

    handleChange = event => {
        const name = event.target.name;
        const value = event.target.value;

        const updatedControls = this.state.formControls;

        const updatedFormElement = updatedControls[name];

        updatedFormElement.value = value;
        updatedFormElement.touched = true;
        updatedFormElement.valid = validate(value, updatedFormElement.validationRules);
        updatedControls[name] = updatedFormElement;

        let formIsValid = true;
        for (let updatedFormElementName in updatedControls) {
            formIsValid = updatedControls[updatedFormElementName].valid && formIsValid;
        }

        this.setState({
            formControls: updatedControls,
            formIsValid: formIsValid
        });
    };

    registerDevice(device) {
        return API_DEVICES.createDevice(this.props.userId,device, (result, status, error) => {
            if (result !== null && (status === 200 || status === 201)) {
                console.log("Successfully inserted device with id: " + result);
                this.reloadHandler();
            } else {
                this.setState(({
                    errorStatus: status,
                    error: error
                }));
            }
        });
    }

    handleSubmit() {
        let device = {
            description: this.state.formControls.description.value,
            address: this.state.formControls.address.value,
            maximumHourlyEnergyConsumption: parseInt(this.state.formControls.maximumHourlyEnergyConsumption.value),
            userId: this.props.userId
        };

        console.log(device);
        this.registerDevice(device);
    }

    render() {
        return (
            <div>
                
                <FormGroup id='description'>
                    <Label for='descriptionField'> Description: </Label>
                    <Input name='description' id='descriptionField' placeholder={this.state.formControls.description.placeholder}
                           onChange={this.handleChange}
                           defaultValue={this.state.formControls.description.value}
                           touched={this.state.formControls.description.touched? 1 : 0}
                           valid={this.state.formControls.description.valid}
                           required
                    />
                    {this.state.formControls.description.touched && !this.state.formControls.description.valid &&
                    <div className={"error-message"}> * Description must have at least 5 characters </div>}
                </FormGroup>

                <FormGroup id='address'>
                    <Label for='addressField'> Address: </Label>
                    <Input name='address' id='addressField' placeholder={this.state.formControls.address.placeholder}
                        onChange={this.handleChange}
                        defaultValue={this.state.formControls.address.value}
                        touched={this.state.formControls.address.touched? 1 : 0}
                        valid={this.state.formControls.address.valid}
                        required
                    />
                    {this.state.formControls.address.touched && !this.state.formControls.address.valid &&
                    <div className={"error-message"}> * Address must have at least 5 characters </div>}
                </FormGroup>

                <FormGroup id='maximumHourlyEnergyConsumption'>
                    <Label for='maxHourlyEnergyField'> Max Hourly Energy Consumption: </Label>
                    <Input type="number" name='maximumHourlyEnergyConsumption' id='maxHourlyEnergyField' placeholder={this.state.formControls.maximumHourlyEnergyConsumption.placeholder}
                        onChange={this.handleChange}
                        defaultValue={this.state.formControls.maximumHourlyEnergyConsumption.value}
                        touched={this.state.formControls.maximumHourlyEnergyConsumption.touched? 1 : 0}
                        valid={this.state.formControls.maximumHourlyEnergyConsumption.valid}
                        required
                    />
                </FormGroup>


                <Row>
                    <Col sm={{size: '4', offset: 8}}>
                        <Button type={"submit"} disabled={!this.state.formIsValid} onClick={this.handleSubmit}> Submit </Button>
                    </Col>
                </Row>

                {
                    this.state.errorStatus > 0 &&
                    <APIResponseErrorMessage errorStatus={this.state.errorStatus} error={this.state.error}/>
                }
            </div>
        );
    }
}

export default DeviceForm;
