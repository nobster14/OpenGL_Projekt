using LearnOpenTK.Common;
using ObjLoader.Loader.Data;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK_Tutorial.DataStructures
{
    public enum ShadersOptions
    {
        phong,
        gouraud,
        constant
    }
    public static class Shaders
    {
        public static void Render(Shader Shader, Camera camera, MyObject data, List<Vector3> pointLights, DirectLight sun, float fogDensity, List<SpotLight> spotLights)
        {
            Shader.SetMatrix4("model", data.model);
            Shader.SetMatrix4("view", camera.GetViewMatrix());
            Shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            Shader.SetVector3("viewPos", camera.Position);

            Shader.SetVector3("material.ambient", data.material.AmbientColor.Map());
            Shader.SetVector3("material.diffuse", data.material.DiffuseColor.Map());
            Shader.SetVector3("material.specular", data.material.SpecularColor.Map());
            Shader.SetFloat("material.shininess", 32.0f);

            Shader.SetVector3("dirLight.direction", sun.direction);
            Shader.SetVector3("dirLight.ambient", sun.ambient);
            Shader.SetVector3("dirLight.diffuse", sun.diffuse);
            Shader.SetVector3("dirLight.specular", sun.specular);

            Shader.SetVector3("fogParams.color", new Vector3(0.74f, 0.929f, 0.78f));
            Shader.SetFloat("fogParams.density", 0);

            for (int i = 0; i < spotLights.Count; i++)
            {
                Shader.SetVector3($"spotLights[{i}].position", spotLights[i].position);
                Shader.SetVector3($"spotLights[{i}].direction", spotLights[i].direction);
                Shader.SetVector3($"spotLights[{i}].ambient", new Vector3(0.0f, 0.0f, 0.0f));
                Shader.SetVector3($"spotLights[{i}].diffuse", new Vector3(1.0f, 1.0f, 1.0f));
                Shader.SetVector3($"spotLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                Shader.SetFloat($"spotLights[{i}].constant", 1.0f);
                Shader.SetFloat($"spotLights[{i}].linear", 0.09f);
                Shader.SetFloat($"spotLights[{i}].quadratic", 0.032f);
                Shader.SetFloat($"spotLights[{i}].cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
                Shader.SetFloat($"spotLights[{i}].outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
            }

            for (int i = 0; i < pointLights.Count; i++)
            {
                Shader.SetVector3($"pointLights[{i}].position", pointLights[i]);
                Shader.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                Shader.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                Shader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                Shader.SetFloat($"pointLights[{i}].constant", 1.0f);
                Shader.SetFloat($"pointLights[{i}].linear", 0.09f);
                Shader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
            }
        }
    }
}
