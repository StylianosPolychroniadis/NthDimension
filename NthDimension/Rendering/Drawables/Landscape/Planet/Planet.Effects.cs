/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/


// Atmo Scattering
/* As found in AtmoPlanet.js
 

(function main() {

    var vec3 = THREE.Vector3;

    var container, stats, controls;
    var scene, light, camera, renderer;

    var atmosphere, c, diffuse, diffuseNight, f, fragmentGround, fragmentSky, g, ground, maxAnisotropy, radius, render, sky, uniforms, vertexGround, vertexSky;

    // scene
    container = document.getElementById('canvas-container');
    scene = new THREE.Scene();

    // camera
    camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 10000);
    // camera orbit control
    cameraCtrl = new THREE.OrbitControls(camera, container);
    cameraCtrl.object.position.z = 200;
    cameraCtrl.update();

    // renderer
    renderer = new THREE.WebGLRenderer({ antialias: true, alpha: false });
    renderer.setSize(window.innerWidth, window.innerHeight);
    container.appendChild(renderer.domElement);

    stats = new Stats();
    container.appendChild(stats.domElement);

    // grid & axis helper
    // var grid = new THREE.GridHelper(600, 50);
    // grid.setColors(0x00bbff, 0xffffff);
    // grid.material.opacity = 0.1;
    // grid.material.transparent = true;
    // grid.position.y = -300;
    // scene.add(grid);

    // var axisHelper = new THREE.AxisHelper(50);
    // scene.add(axisHelper);



    // --- Main

    var scene_settings = {
        bgColor: 0x000000
    };


    radius = 100.0;

    atmosphere = {

        Kr: 0.0008,	// 0.0025
        Km: 0.001,	// 0.0010
        ESun: 30.0,	// 20
        g: -0.950,
        innerRadius: 100,
        outerRadius: 103,
        wavelength: [0.95, 0.54, 0.47],
        scaleDepth: 0.41,	// 0.25
        mieScaleDepth: 0.1,
        wr: 0.95,
        wg: 0.6,
        wb: 0.47

    };

    var dTexLoaded = false;
    diffuse = THREE.ImageUtils.loadTexture('img/earth.jpg', new THREE.UVMapping(), function () {
        // console.log(diffuse);
    });

    diffuseNight = THREE.ImageUtils.loadTexture('img/night.jpg', new THREE.UVMapping(), function () {
        // console.log(diffuseNight);
    });

    diffuseCloud = THREE.ImageUtils.loadTexture('img/cloud.jpg', new THREE.UVMapping(), function () {
        diffuseCloud.premultiplyAlpha = true;
        diffuseCloud.needsUpdate = true;
        // console.log(diffuseCloud);
    });


    maxAnisotropy = renderer.getMaxAnisotropy();
    diffuse.anisotropy = maxAnisotropy;
    diffuseNight.anisotropy = maxAnisotropy;

    uniforms = {
        v3LightPosition: {
            type: "v3",
            value: new THREE.Vector3(1e8, 0, 1e8).normalize()
        },
        v3InvWavelength: {
            type: "v3",
            value: new THREE.Vector3(1 / Math.pow(atmosphere.wr, 4), 1 / Math.pow(atmosphere.wg, 4), 1 / Math.pow(atmosphere.wb, 4))
        },
        fCameraHeight: {
            type: "f",
            value: 0
        },
        fCameraHeight2: {
            type: "f",
            value: 0
        },
        fInnerRadius: {
            type: "f",
            value: atmosphere.innerRadius
        },
        fInnerRadius2: {
            type: "f",
            value: atmosphere.innerRadius * atmosphere.innerRadius
        },
        fOuterRadius: {
            type: "f",
            value: atmosphere.outerRadius
        },
        fOuterRadius2: {
            type: "f",
            value: atmosphere.outerRadius * atmosphere.outerRadius
        },
        fKrESun: {
            type: "f",
            value: atmosphere.Kr * atmosphere.ESun
        },
        fKmESun: {
            type: "f",
            value: atmosphere.Km * atmosphere.ESun
        },
        fKr4PI: {
            type: "f",
            value: atmosphere.Kr * 4.0 * Math.PI
        },
        fKm4PI: {
            type: "f",
            value: atmosphere.Km * 4.0 * Math.PI
        },
        fScale: {
            type: "f",
            value: 1 / (atmosphere.outerRadius - atmosphere.innerRadius)
        },
        fScaleDepth: {
            type: "f",
            value: atmosphere.scaleDepth
        },
        fScaleOverScaleDepth: {
            type: "f",
            value: 1 / (atmosphere.outerRadius - atmosphere.innerRadius) / atmosphere.scaleDepth
        },
        g: {
            type: "f",
            value: atmosphere.g
        },
        g2: {
            type: "f",
            value: atmosphere.g * atmosphere.g
        },
        nSamples: {
            type: "i",
            value: 3
        },
        fSamples: {
            type: "f",
            value: 3.0
        },
        tDiffuse: {
            type: "t",
            value: diffuse
        },
        tDiffuseNight: {
            type: "t",
            value: diffuseNight
        },
        tDisplacement: {
            type: "t",
            value: 0
        },
        tSkyboxDiffuse: {
            type: "t",
            value: 0
        },
        fNightScale: {
            type: "f",
            value: 1
        },
        time: {
            type: "f",
            value: 0.0
        }
    };

    ground = {
        geometry: new THREE.SphereGeometry(atmosphere.innerRadius, 100, 100),
        material: new THREE.ShaderMaterial({
            uniforms: uniforms,
            vertexShader: document.getElementById('vertexGround').textContent,
            fragmentShader: document.getElementById('fragmentGround').textContent,
        })
    };

    ground.mesh = new THREE.Mesh(ground.geometry, ground.material);
    scene.add(ground.mesh);


    sky = {
        geometry: new THREE.SphereGeometry(atmosphere.outerRadius, 200, 200),
        material: new THREE.ShaderMaterial({
            uniforms: uniforms,
            vertexShader: document.getElementById('vertexSky').textContent,
            fragmentShader: document.getElementById('fragmentSky').textContent,
            side: THREE.BackSide,
            transparent: true,
            blending: THREE.AdditiveBlending,
        })
    };

    console.log(sky.material);

    sky.mesh = new THREE.Mesh(sky.geometry, sky.material);
    scene.add(sky.mesh);



    var cloud = {
        geometry: new THREE.SphereGeometry(atmosphere.innerRadius + 1, 100, 100),
        material: new THREE.MeshLambertMaterial({
            map: diffuseCloud,
            // blendSrc: THREE.SrcAlphaFactor,
            // blendDst: THREE.SrcAlphaFactor,
            // blending: THREE.CustomBlending,
            blending: THREE.AdditiveBlending,
            transparent: true
        })
    };

    cloud.mesh = new THREE.Mesh(cloud.geometry, cloud.material);
    scene.add(cloud.mesh);




    // ---------- GUI ----------

    var gui = new dat.GUI();
    gui.width = 300;

    var gui_settings = gui.addFolder('Settings');

    gui_settings.add(atmosphere, 'scaleDepth', 0.0, 1.0, 0.01).name('Scale Depth');
    gui_settings.add(atmosphere, 'Kr', 0.0, 0.005, 0.0001).name('Kr');
    gui_settings.add(atmosphere, 'Km', 0.0, 0.005, 0.0001).name('Km');
    gui_settings.add(atmosphere, 'ESun', 0.0, 200.0, 1.0).name('ESun');

    gui_settings.add(atmosphere, 'wr', 0.0, 1.0, 0.001).name('wRed');
    gui_settings.add(atmosphere, 'wg', 0.0, 1.0, 0.001).name('wGreen');
    gui_settings.add(atmosphere, 'wb', 0.0, 1.0, 0.001).name('wBlue');

    // gui_settings.add(atmosphere, 'g', -20.0, 20.0, 0.1).name('g');

    gui_settings.addColor(scene_settings, 'bgColor').name('Background');

    gui_settings.open();

    for (var i in gui_settings.__controllers) {
        gui_settings.__controllers[i].onChange(updateShaderUniforms);
    }

    function updateShaderUniforms() {

        uniforms.fKrESun.value = atmosphere.Kr * atmosphere.ESun;
        uniforms.fKr4PI.value = atmosphere.Kr * 4.0 * Math.PI;

        uniforms.fKmESun.value = atmosphere.Km * atmosphere.ESun;
        uniforms.fKm4PI.value = atmosphere.Km * 4.0 * Math.PI;

        uniforms.fScaleDepth.value = atmosphere.scaleDepth;
        uniforms.fScaleOverScaleDepth.value = 1 / (atmosphere.outerRadius - atmosphere.innerRadius) / atmosphere.scaleDepth;

        uniforms.fKrESun.value = atmosphere.Kr * atmosphere.ESun;
        uniforms.fKmESun.value = atmosphere.Km * atmosphere.ESun;

        uniforms.v3InvWavelength.value.set(1 / Math.pow(atmosphere.wr, 4), 1 / Math.pow(atmosphere.wg, 4), 1 / Math.pow(atmosphere.wb, 4));

        // uniforms.g.value = atmosphere.g;
        // uniforms.g2.value = atmosphere.g * atmosphere.g;


        // ground.material.needsUpdate = true;
        // sky.material.needsUpdate = true;

    }


    // ---------- end GUI ----------


    var directionalLight = new THREE.DirectionalLight(0xffffff, 0.95);
    directionalLight.position.set(1, 0, 0);
    scene.add(directionalLight);

    var ambientLight = new THREE.AmbientLight(0x101010);
    scene.add(ambientLight);




    var test = {
        material: new THREE.MeshBasicMaterial({ color: 0x00aaff, blending: THREE.AdditiveBlending }),
        geometry: new THREE.SphereGeometry(300, 100, 100)
    };

    test.mesh = new THREE.Mesh(test.geometry, test.material);
    test.mesh.position.x = 1000;
    scene.add(test.mesh);



    // transparent is the problem, try uncomment it!
    // var test2 = {
    // 	material: new THREE.MeshBasicMaterial( {color: 0xffffff, wireframe: false, transparent: true, opacity: 1.0, side: THREE.BackSide} ),
    // 	geometry: new THREE.SphereGeometry(3000, 24, 24)
    // };

    // test2.mesh = new THREE.Mesh(test2.geometry, test2.material);
    // scene.add(test2.mesh);




    var theta = -0.005;
    light = new THREE.Vector3(1, 0, 0).normalize();

    (function run() {

        requestAnimationFrame(run);
        renderer.setClearColor(scene_settings.bgColor, 1);

        var cameraHeight, euler, eye, matrix, vector;

        uniforms.time.value += 0.001;


        // light.applyAxisAngle(new THREE.Vector3(0, 1, 0), theta);
        // directionalLight.position.set(light.x, light.y, light.z);
        // test.mesh.position = light.clone().multiplyScalar(5000);


        cloud.mesh.rotation.y += 0.0022;
        ground.mesh.rotation.y += 0.002;
        sky.mesh.rotation.y += 0.002;

        if (dTexLoaded) {
            var time = Date.now() * 0.00025;
        }


        cameraHeight = camera.position.length();
        sky.material.uniforms.v3LightPosition.value = light;
        sky.material.uniforms.fCameraHeight.value = cameraHeight;
        sky.material.uniforms.fCameraHeight2.value = cameraHeight * cameraHeight;
        ground.material.uniforms.v3LightPosition.value = light;
        ground.material.uniforms.fCameraHeight.value = cameraHeight;
        ground.material.uniforms.fCameraHeight2.value = cameraHeight * cameraHeight;

        renderer.render(scene, camera);
        stats.update();

    })();



    // browser events
    window.addEventListener('keypress', function (event) {
        if (event.keyCode === 32) {	// if spacebar is pressed
            event.preventDefault();
            scene_settings.pause = !scene_settings.pause;
        }
    });

    window.addEventListener('resize', onWindowResize, false);
    function onWindowResize() {
        var w = window.innerWidth;
        var h = window.innerHeight;
        camera.aspect = w / h;
        camera.updateProjectionMatrix();
        renderer.setSize(w, h);
    }

}());


 






*/
